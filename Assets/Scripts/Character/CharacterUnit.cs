using AYellowpaper.SerializedCollections;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.U2D.Animation;

public class CharacterUnit : PoolableMono
{
    public SpriteLibrary SpriteLibrary => spriteLibrary;

    public string CustomStateGroupAddress => customStateGroupAddress;

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

    public void SetSpriteLibraryAsset(SpriteLibraryAsset spriteLibraryAsset)
    {
        if (spriteLibrary != null)
            spriteLibrary.spriteLibraryAsset = spriteLibraryAsset;
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
    private CharacterAnimationEffect characterAnimationEffect;

    [SerializeField]
    protected ScriptableCharacterStat baseStat;

    [SerializeField]
    protected SerializedDictionary<CharacterAnimState, float> animDelayDic;

    [SerializeField]
    protected SerializedDictionary<CharacterAnimState, CharacterAnimState> alternativeAnimDic;

    [SerializeField]
    private string customStateGroupAddress;

    [SerializeField]
    private bool debugLog;

    private ScriptableCharacterStateGroup stateGroup;
    private ScriptableCharacterModuleGroup moduleGroup;
    private ScriptableCharacterState defaultState;
    private bool activated = false;
    private int lastAnimStateHash = -1;
    private float updateTimer = 0;

    private CharacterActionHandler actionHandler;

    private Dictionary<ModuleType, IModuleModel> moduleModelDic;

    private Dictionary<int, CharacterAnimState> shortNameHashDic = new();

    private void Update()
    {
        if (!activated)
            return;

        UpdateSubModules();

        if (CheckUpdatableState())
            CheckNextState();
    }

    private void FixedUpdate()
    {
        if (!activated)
            return;

        UpdatePhysics();
        ProcessCurrentState();
    }

    public virtual void Initialize()
    {
        InitializeAnimState();
        InitializeModel();
        InitializeComponent();
        InitializeValues();
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

        if (characterCollider2D)
            characterCollider2D.enabled = true;

        if (triggerCollider2D)
            triggerCollider2D.enabled = true;
        
        // 우선순위가 높은 순으로 정렬하므로
        // 가장 마지막에 있는 것이 기본 상태가 된다.
        stateGroup.Sort();
        defaultState = stateGroup.StateList.Last();

        // 기본 상태 설정
        ChangeState(defaultState);

        // 전투 이벤트 시작
        if (Model.BattleEventProcessor != null)
            Model.BattleEventProcessor.Start();

        // 어빌리티 처리 시작
        if (Model.AbilityProcessor != null)
            Model.AbilityProcessor.Start();

        activated = true;

        Model.SetIsActivate(true);
        gameObject.SafeSetActive(true);
    }

    public void StopUpdate()
    {
        activated = false;

        if (Model != null)
            Model.SetIsActivate(activated);
    }

    private void OnDeactivate()
    {
        activated = false;

        if (Model != null)
            Model.SetIsActivate(activated);

        if (characterCollider2D)
            characterCollider2D.enabled = false;

        if (triggerCollider2D)
            triggerCollider2D.enabled = false;

        moduleGroup.OnEventCharacterDeactivate(Model, moduleModelDic);

        if (Model.AbilityProcessor != null)
            Model.AbilityProcessor.Cancel();

        if (Model.BattleEventProcessor != null)
            Model.BattleEventProcessor.Cancel();

        Model.ActionHandler.Cancel();

        NotifyDeadCharacter();

        // 거리가 멀다면 죽음 애니 생략
        if (Model.DistanceToTarget >= DistanceToTarget.Far)
        {
            gameObject.SafeSetActive(false);
        }
        else
        {
            DeadAnimAsync().Forget();
        }
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

        if (characterCollider2D)
            characterCollider2D.enabled = false;

        if (triggerCollider2D)
            triggerCollider2D.enabled = false;

        if (animator)
            animator.updateMode = AnimatorUpdateMode.Normal;
    }

    private void InitializeValues()
    {
        moduleModelDic = moduleGroup.CreateModuleModelDic();
        lastAnimStateHash = -1;
        updateTimer = 0;
    }

    protected virtual void UpdateSubModules()
    {
        RefreshAnimState();

        moduleGroup.OnEventUpdate(Model, moduleModelDic);

        if (Model.BattleEventProcessor != null)
            Model.BattleEventProcessor.Update();

        if (Model.AbilityProcessor != null)
            Model.AbilityProcessor.Update();

        // 활성 상태 동기화
        Model.SetIsActivate(activated);
    }

    protected void ProcessCurrentState()
    {
        if (Model == null)
            return;

        if (Model.BattleEventProcessor != null)
        {
            bool isOnCC = Model.BattleEventProcessor.IsCrowdControl();

            if (isOnCC)
                return;
        }

        CurrentState.OnStateAction(Model);
    }

    protected void CheckNextState()
    {
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
    }

    private bool CheckUpdatableState()
    {
        // 즉시 Dead 상태 전환 필요
        if (Model.IsDead)
            return true;

        if (Model.GetUpdateInterval() > 0)
        {
            updateTimer += Time.deltaTime;

            if (updateTimer < Model.GetUpdateInterval())
                return false;

            updateTimer = 0;
        }

        if (Model.BattleEventProcessor != null)
        {
            bool isOnCC = Model.BattleEventProcessor.IsCrowdControl();

            if (isOnCC)
                return false;
        }
        
        return true;
    }

    protected void UpdatePhysics()
    {
        if (!Model.IsEnablePhysics)
        {
            rigidBody2D.linearVelocity = Vector2.zero;
            rigidBody2D.angularVelocity = 0;
        }
    }

    private void SelectState()
    {
        var stateList = stateGroup.StateList;
        
        for (int i = 0; i < stateList.Count; i++)
        {
            var state = stateList[i];
            
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
        AnimatorStateInfo currentStateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (lastAnimStateHash == currentStateInfo.shortNameHash)
            return;

        if (shortNameHashDic.TryGetValue(currentStateInfo.shortNameHash, out CharacterAnimState animState))
            Model.SetCurrentAnimState(animState);

        lastAnimStateHash = currentStateInfo.shortNameHash;
    }

    private ScriptableCharacterState FindCandidateState()
    {
        var currentPriority = CurrentState.Priority;
        var stateList = stateGroup.StateList;
        
        for (int i = 0; i < stateList.Count; i++)
        {
            var state = stateList[i];
            
            if (state == null || state == CurrentState)
                continue;

            if (state.Priority <= currentPriority)
                continue;

            if (state.CheckEnterCondition(Model))
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

        var resolvedAnimState = ResolveAnimStateParameter(state.AnimState);
        animator.SetInteger(StringDefine.CHARACTER_ANIM_STATE_KEY, resolvedAnimState);
        
        CurrentState.OnEnterState(Model);
        Model.SetStateEnterTime(Time.time);

        if (debugLog)
            Logger.Log($"[{name}] Current State : {state}");
    }

    private void InitializeModel()
    {
        Model.SetActionHandler(CreateActionHandler());
        Model.InitializeStat(baseStat);
        Model.SetTransform(transform);
        Model.SetAgent(agent);
        Model.SetIsActivate(false);
        Model.SetDistanceToTarget(DistanceToTarget.Close);
        Model.SetAnimDelayDic(animDelayDic);
    }

    private CharacterActionHandler CreateActionHandler()
    {
        var pathFinder = CreatePathFinder();

        if (actionHandler == null)
            actionHandler = new CharacterActionHandler(animator, rigidBody2D, bodySprite, gameObject, pathFinder);
        
        actionHandler.SetOnFlipX(OnFlipX);
        actionHandler.SetOnDeactivate(OnDeactivate);

        if (characterAnimationEffect != null)
            actionHandler.SetCharacterAnimEffect(characterAnimationEffect);

        return actionHandler;
    }

    private void InitializeAnimState()
    {
        var values = (CharacterAnimState[])System.Enum.GetValues(typeof(CharacterAnimState));

        for (int i = 0; i < values.Length; i++)
        {
            CharacterAnimState state = values[i];

            if (state == CharacterAnimState.None)
                continue;

            var stringHash = Animator.StringToHash(state.ToString());

            if (alternativeAnimDic != null &&
                alternativeAnimDic.TryGetValue(state, out var alterAnim))
                state = alterAnim;

            shortNameHashDic[Animator.StringToHash(state.ToString())] = state;
        }
    }

    // 실제 애니메이터에 보낼 파라미터
    private int ResolveAnimStateParameter(CharacterAnimState animState)
    {
        if (animState == CharacterAnimState.Attack)
        {
            var weapon = Model.GetEquipment(EquipmentType.Weapon);

            if (weapon != null)
                return (int)weapon.AttackAnimState;
        }

        if (alternativeAnimDic != null &&
            alternativeAnimDic.TryGetValue(animState, out var alterAnim))
            return (int)alterAnim;

        return (int)animState;
    }

    private IPathFinder CreatePathFinder()
    {
        switch (Model.PathFindType)
        {
            case PathFindType.Navmesh:
                NavmeshPathFinder navmeshPathFinder = new NavmeshPathFinder(transform, agent);
                return navmeshPathFinder;

            case PathFindType.AStar:
                AStarPathFinder aStarPathFinder = new AStarPathFinder(
                Model.OnFindAStarNodes,
                rigidBody2D,
                FloatDefine.ASTAR_NEXT_NODE_THRESHOLD,
                OnGetMoveSpeed);

                return aStarPathFinder;
        }

        return null;
    }

    private float OnGetMoveSpeed()
    {
        return Model.GetStatValue(StatType.MoveSpeed);
    }

    private async UniTask DeadAnimAsync()
    {
        if (animator == null)
            return;

        await UniTaskUtils.WaitForLastUpdate();

        var deadStateInfo = animator.GetCurrentAnimatorStateInfo(0);

        while (gameObject.SafeActiveSelf() && deadStateInfo.normalizedTime >= 1f)
            await UniTaskUtils.NextFrame();

        if (bodySprite)
            bodySprite.DeactiveWithFade(1, gameObject);
    }

    private void NotifyDeadCharacter()
    {
        var battleObserverParam = new BattleObserverParam();
        battleObserverParam.SetIntValue(gameObject.GetInstanceID());
        battleObserverParam.SetModelValue(Model);

        ObserverManager.NotifyObserver(BattleObserverID.DeadCharacter, battleObserverParam);
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
