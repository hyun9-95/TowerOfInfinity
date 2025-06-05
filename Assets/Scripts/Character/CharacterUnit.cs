using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class CharacterUnit : PoolableMono
{
    public CharacterUnitModel Model { get; private set; }

    public void SetModel(CharacterUnitModel model)
    {
        Model = model;
    }

    public void SetStateGroup(ScriptableCharacterStateGroup stateGroup)
    {
        this.stateGroup = stateGroup;
    }

    public void SetModuleGroup(ScriptableCharacterModuleGroup moduleGroup)
    {
        this.moduleGroup = moduleGroup;
    }

    protected ScriptableCharacterState CurrentState { get; private set; }

    [SerializeField]
    protected Animator animator;

    [SerializeField]
    protected Rigidbody2D rigidBody2D;

    [SerializeField]
    protected SpriteRenderer bodySprite;

    [SerializeField]
    protected NavMeshAgent agent;

    [SerializeField]
    protected Collider2D characterCollider2D;

    [SerializeField]
    protected ScriptableCharacterStat baseStat;

    [SerializeField]
    protected SerializableDictionary<CharacterAnimState, CharacterAnimState> animStateResolver;

    [Tooltip("경로를 다시 계산하기까지 기다리는 시간")]
    [SerializeField]
    protected float repathCoolTime = 0.25f;

    [Tooltip("현재 노드에 도착했는지 판단할 거리")]
    [SerializeField]
    protected float nextNodeThreshold = 0.5f;

    [SerializeField]
    private bool debugLog;

    private ScriptableCharacterStateGroup stateGroup;
    private ScriptableCharacterModuleGroup moduleGroup;
    private ScriptableCharacterState defaultState;
    private bool activated = false;

    private static readonly BattleEventProcessor battleEventProcessor = new BattleEventProcessor();
    private static readonly WeaponProcessor weaponProcessor = new WeaponProcessor();

    private Dictionary<ModuleType, IModuleModel> moduleModelDic;

    private Dictionary<CharacterAnimState, CharacterAnimState> reversedAnimStateResolver = null;
    private Dictionary<int, CharacterAnimState> shortNameHashDic = new Dictionary<int, CharacterAnimState>();

    protected virtual void Update()
    {
        if (!activated)
            return;

        PrepareState();
    }

    private void LateUpdate()
    {
        if (!activated)
            return;

        UpdateState();
    }

    public virtual void Initialize()
    {
        InitializeAnimState();
        InitializeModel();
        InitializeComponent();
        InitializeModule();
    }

    /// <summary>
    /// 초기화 이후 호출해야한다.
    /// </summary>
    public virtual void Activate()
    {
        if (Model == null)
        {
            Logger.Null($"Model is null ! {gameObject.name}");
            return;
        }

        // 우선순위대로 정렬하므로
        // 가장 마지막에 있는 것이 기본 상태가 된다.
        stateGroup.Sort();
        defaultState = stateGroup.StateList.Last();
        ChangeState(defaultState);

        gameObject.SafeSetActive(true);

        bodySprite.RestoreAlpha();
        activated = true;
    }

    protected virtual void OnDeactivate()
    {
        activated = false;

        moduleGroup.OnEventUpdate(Model, moduleModelDic);

        if (Model.IsDead)
            weaponProcessor.Cancel(Model);
    }

    private void OnFlipX(bool isFlip)
    {
        Model.SetIsFlipX(isFlip);
    }

    private void InitializeComponent()
    {
        if (Model.PathFindType == PathFindType.Navmesh)
        {
            if (agent == null)
                return;

            if (agent.isOnNavMesh)
            {
                // 2D로 사용시 로테이션 돌아가는 것 방지
                agent.updateUpAxis = false;
                agent.updateRotation = false;
                transform.rotation = Quaternion.identity;

                agent.enabled = true;
            }
            else
            {
                agent.enabled = false;
            }
        }
        else if (Model.PathFindType == PathFindType.AStar)
        {
            if (agent != null)
                agent.enabled = false;
        }

        if (Model.TeamTag == TeamTag.Ally)
        {
            rigidBody2D.bodyType = RigidbodyType2D.Kinematic;
            characterCollider2D.isTrigger = true;
        }
        else
        {
            rigidBody2D.bodyType = RigidbodyType2D.Dynamic;
            characterCollider2D.isTrigger = false;
        }
    }

    private void InitializeModule()
    {
        moduleModelDic = moduleGroup.CreateModuleModelDic();
    }

    protected virtual void PrepareState()
    {
        RefreshAnimState();

        moduleGroup.OnEventUpdate(Model, moduleModelDic);
        battleEventProcessor.Process(Model);
        weaponProcessor.Process(Model);
    }

    protected void UpdateState()
    {
        if (Model == null)
            return;

        if (CurrentState == defaultState || CurrentState.CheckExitCondition(Model))
        {
            SelectState();
        }
        else
        {
            var candidateState = FindCandidateState();

            if (candidateState != null)
                ChangeState(candidateState);
        }

        CurrentState.OnStateAction(Model);
    }

    private void SelectState()
    {
        foreach (ScriptableCharacterState state in stateGroup.StateList)
        {
            if (state == null || state == CurrentState)
                continue;

            if (state.CheckEnterCondition(Model))
            {
                ChangeState(state);
                break;
            }
        }
    }

    private void RefreshAnimState()
    {
        var currentStateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (shortNameHashDic.TryGetValue(currentStateInfo.shortNameHash, out CharacterAnimState animState))
        {
            if (reversedAnimStateResolver != null)
            {
                var reversedAnimState = ReverseAnimState(animState);
                Model.SetCurrentAnimState(reversedAnimState);
            }
            else
            {
                Model.SetCurrentAnimState(animState);
            }
        }
    }

    private ScriptableCharacterState FindCandidateState()
    {
        foreach (ScriptableCharacterState state in stateGroup.StateList)
        {
            if (state == null || state == CurrentState)
                continue;

            if (state.CheckEnterCondition(Model) && CurrentState.Priority < state.Priority)
                return state;
        }

        return null;
    }

    /// <summary>
    /// 상태 변환은 여기서만!
    /// </summary>
    /// <param name="state"></param>
    protected void ChangeState(ScriptableCharacterState state)
    {
        if (CurrentState == state)
            return;

        if (CurrentState != null && CurrentState != state)
            CurrentState.OnExitState(Model);

        CurrentState = state;

        var resolvedAnimState = ResolveAnimState(state.AnimState);
        animator.SetInteger(StringDefine.CHARACTER_ANIM_STATE_KEY, resolvedAnimState);

        CurrentState.OnEnterState(Model);

        if (debugLog)
            Logger.Log($"[{name}] Current State : {state}");
    }

    private void InitializeModel()
    {
        Model.SetActionHandler(CreateActionHandler());
        Model.SetBaseStat(baseStat);
        Model.SetTransform(transform);
        Model.SetAgent(agent);
    }

    private CharacterStateActionHandler CreateActionHandler()
    {
        var pathFinder = CreatePathFinder();
        var actionHandler = new CharacterStateActionHandler(animator, rigidBody2D, bodySprite, gameObject, pathFinder);
        actionHandler.SetOnDeactivate(OnDeactivate);
        actionHandler.SetOnFlipX(OnFlipX);

        return actionHandler;
    }

    private void InitializeAnimState()
    {
        if (animStateResolver.Count > 0)
            reversedAnimStateResolver = animStateResolver.ToDictionary(pair => pair.Value, pair => pair.Key);

        var values = (CharacterAnimState[])System.Enum.GetValues(typeof(CharacterAnimState));

        for (int i = 0; i < values.Length; i++)
        {
            CharacterAnimState state = values[i];
            shortNameHashDic[Animator.StringToHash(state.ToString())] = state;
        }
    }


    private int ResolveAnimState(CharacterAnimState animState)
    {
        if (animStateResolver == null || !animStateResolver.ContainsKey(animState))
            return (int)animState;

        return (int)animStateResolver[animState];
    }

    private CharacterAnimState ReverseAnimState(CharacterAnimState animState)
    {
        if (reversedAnimStateResolver.TryGetValue(animState, out CharacterAnimState reversedAnimState))
            return reversedAnimState;

        return animState;
    }

    private IPathFinder CreatePathFinder()
    {
        if (Model.PathFindType == PathFindType.Navmesh)
        {
            NavmeshPathFinder navmeshPathFinder = new NavmeshPathFinder(transform, agent);
            return navmeshPathFinder;
        }
        else
        {
            AStarPathFinder aStarPathFinder = new AStarPathFinder(rigidBody2D, repathCoolTime, nextNodeThreshold,
                OnGetMoveSpeed, OnGetAStarPaths);
            return aStarPathFinder;
        }
    }

    private float OnGetMoveSpeed()
    {
        return Model.GetStatValue(StatType.MoveSpeed);
    }

    private List<AStarNode> OnGetAStarPaths(Vector3 start, Vector3 pos)
    {
        return AStarManager.Instance.FindPath(start, pos);
    }

    #region Collision
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!activated)
            return;

        moduleGroup.OnEventTriggerEnter2D(collision, Model, moduleModelDic);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!activated)
            return;

        moduleGroup.OnEventTriggerStay2D(collision, Model, moduleModelDic);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!activated)
            return;

        moduleGroup.OnEventTriggerExit2D(collision, Model, moduleModelDic);
    }
    #endregion
}
