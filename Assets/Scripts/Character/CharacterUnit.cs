using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.U2D.Animation;

public class CharacterUnit : PoolableMono
{
    public SpriteLibrary SpriteLibrary => spriteLibrary;

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
    protected SpriteLibrary spriteLibrary;

    [SerializeField]
    protected NavMeshAgent agent;

    [SerializeField]
    protected Collider2D characterCollider2D;

    [SerializeField]
    protected Collider2D triggerCollider2D;

    [SerializeField]
    protected ScriptableCharacterStat baseStat;

    [SerializeField]
    protected SerializableDictionary<CharacterAnimState, CharacterAnimState> animStateResolver;

    [SerializeField]
    private bool debugLog;

    private ScriptableCharacterStateGroup stateGroup;
    private ScriptableCharacterModuleGroup moduleGroup;
    private ScriptableCharacterState defaultState;
    private bool activated = false;

    private BattleEventProcessor battleEventProcessor = new BattleEventProcessor();
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
        UpdatePhysics();
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

        bodySprite.RestoreAlpha();
        characterCollider2D.enabled = true;
        triggerCollider2D.enabled = true;

        // 우선순위대로 정렬하므로
        // 가장 마지막에 있는 것이 기본 상태가 된다.
        stateGroup.Sort();
        defaultState = stateGroup.StateList.Last();
        ChangeState(defaultState);

        activated = true;
        gameObject.SafeSetActive(true);

        // 패시브 스킬 활성화
        if (Model.PassiveSkill != null)
            Model.PassiveSkill.Activate();
    }

    private void OnDeactivate()
    {
        activated = false;

        characterCollider2D.enabled = false;
        triggerCollider2D.enabled = false;

        moduleGroup.OnEventUpdate(Model, moduleModelDic);

        if (Model.IsDead)
            weaponProcessor.Cancel(Model);

        Model.ActionHandler.Cancel();

        DeadAsync().Forget();
    }

    private void OnFlipX(bool isFlip)
    {
        Model.SetIsFlipX(isFlip);
    }

    private void InitializeComponent()
    {
        if (Model.PathFindType == PathFindType.Navmesh)
        {
            if (agent != null)
            {
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
        }
        else if (Model.PathFindType == PathFindType.AStar)
        {
            if (agent != null)
                agent.enabled = false;
        }

        characterCollider2D.enabled = false;
        triggerCollider2D.enabled = false;
    }

    private void InitializeModule()
    {
        moduleModelDic = moduleGroup.CreateModuleModelDic();
    }

    protected virtual void PrepareState()
    {
        RefreshAnimState();

        moduleGroup.OnEventUpdate(Model, moduleModelDic);
        battleEventProcessor.Update();
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

        if (CurrentState is CharacterDeadState)
            OnDeactivate();
    }

    protected void UpdatePhysics()
    {
        if (IsEnablePhysics())
        {
            
        }
        else
        {
            rigidBody2D.linearVelocity = Vector2.zero;
            rigidBody2D.angularVelocity = 0;
        }
    }

    private bool IsEnablePhysics()
    {
        if (battleEventProcessor.IsProcessingBattleEvent(BattleEventType.KnockBack))
            return true;

        return false;
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
        Model.InitializeStat(baseStat);
        Model.SetTransform(transform);
        Model.SetAgent(agent);
 
        battleEventProcessor.SetOwner(Model);
        Model.SetEventProcessorWrapper(new BattleEventProcessorWrapper(battleEventProcessor));
    }

    private CharacterStateActionHandler CreateActionHandler()
    {
        var pathFinder = CreatePathFinder();
        var actionHandler = new CharacterStateActionHandler(animator, rigidBody2D, bodySprite, gameObject, pathFinder);
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
            AStarPathFinder aStarPathFinder = new AStarPathFinder(rigidBody2D, FloatDefine.ASTAR_NEXT_NODE_THRESHOLD,
                OnGetMoveSpeed);
            return aStarPathFinder;
        }
    }

    private float OnGetMoveSpeed()
    {
        return Model.GetStatValue(StatType.MoveSpeed);
    }

    private async UniTask DeadAsync()
    {
        var targetModel = BattleSceneManager.Instance.GetCharacterModel(gameObject.GetInstanceID());
        var expGemModel = targetModel != null && targetModel.TeamTag == TeamTag.Enemy ? new BattleExpGemModel() : null;

        if (BattleSceneManager.Instance != null)
            BattleSceneManager.Instance.RemoveLiveCharacter(gameObject.GetInstanceID());

        await UniTask.WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

        bodySprite.FadeOff(1, gameObject);

        if (expGemModel != null)
        {
            var exp = await ObjectPoolManager.Instance.SpawnPoolableMono<BattleExpGem>(PathDefine.BATTLE_EXP_GEM, gameObject.transform.position, Quaternion.identity);
            exp.SetModel(expGemModel);
            exp.ShowAsync().Forget();
        }
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
