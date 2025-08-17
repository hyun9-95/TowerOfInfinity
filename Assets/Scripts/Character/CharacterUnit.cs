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
    private string customStateGroupAddress;

    [SerializeField]
    private bool debugLog;

    private ScriptableCharacterStateGroup stateGroup;
    private ScriptableCharacterModuleGroup moduleGroup;
    private ScriptableCharacterState defaultState;
    private bool activated = false;
    private bool stateUpdate = false;
    private int lastAnimStateHash = -1;
    private float cameraVisibleDistance = 0;
    private float farDistance = 0;
    private float veryFarDistance = 0;
    private float updateInterval = 0;
    private float updateTimer = 0;

    private AbilityProcessor abilityProcessor = new();
    private BattleEventProcessor battleEventProcessor = new();
    private CharacterActionHandler actionHandler;

    private Dictionary<ModuleType, IModuleModel> moduleModelDic;

    private Dictionary<int, CharacterAnimState> shortNameHashDic = new();

    private void Update()
    {
        if (!activated)
            return;

        UpdatePhysics();
        PrepareState();

        if (stateUpdate)
        {
            ProcessCurrentState();
            CheckNextState();
        }
        else if (Model.IsDead)
        {
            // 죽었다면 즉시 상태 검사
            CheckNextState();
        }
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
        battleEventProcessor.Start();

        // 어빌리티 처리 시작
        abilityProcessor.Start();

        stateUpdate = true;
        activated = true;

        Model.SetIsActivate(true);
        gameObject.SafeSetActive(true);
    }

    public void StopUpdate()
    {
        activated = false;
        stateUpdate = false;

        if (Model != null)
            Model.SetIsActivate(activated);
    }

    public void OnStateUpdateChange(bool value)
    {
        stateUpdate = value;
    }

    public float GetPrimaryWeaponCoolTime()
    {
        if (abilityProcessor == null)
            return 0;

        return abilityProcessor.GetPrimaryWeaponCoolTime();
    }

    private void OnDeactivate(bool deadAsync = true)
    {
        activated = false;
        stateUpdate = false;

        if (Model != null)
            Model.SetIsActivate(activated);

        if (characterCollider2D)
            characterCollider2D.enabled = false;

        if (triggerCollider2D)
            triggerCollider2D.enabled = false;

        moduleGroup.OnEventCharacterDeactivate(Model, moduleModelDic);

        abilityProcessor.Cancel();
        battleEventProcessor.Cancel();
        Model.ActionHandler.Cancel();

        if (deadAsync)
            DeadAsync().Forget();
        else
        {
            BattleSceneManager.Instance.RemoveLiveCharacter(gameObject.GetInstanceID());
            gameObject.SafeSetActive(false);
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
        cameraVisibleDistance = CameraManager.Instance.DiagonalLengthFromCenter + 0.5f;
        farDistance = cameraVisibleDistance * 2;
        veryFarDistance = cameraVisibleDistance * 2.5f;
        lastAnimStateHash = -1;
        updateTimer = 0;
    }

    protected virtual void PrepareState()
    {
        RefreshAnimState();

        moduleGroup.OnEventUpdate(Model, moduleModelDic);

        if (Model.CharacterSetUpType == CharacterSetUpType.Battle)
        {
            abilityProcessor.Update();
            battleEventProcessor.Update();
        }

        // 활성 상태 동기화
        Model.SetIsActivate(activated);

        // 플레이어와의 거리에 따라 업데이트 주기 갱신
        UpdateDistantceToTarget();
    }

    private void UpdateDistantceToTarget()
    {
        var distanceToTarget = DistanceToTarget.Close;

        // 타겟이 있다면 거리 캐싱
        if (Model.Target != null)
        {
            // 거리에 따라 업데이트 주기를 갱신
            float distanceSqr = (Model.Target.Transform.position - gameObject.transform.position).sqrMagnitude;
            Model.SetDistanceToTargetSqr(distanceSqr);

            if (distanceSqr >= (veryFarDistance * veryFarDistance))
            {
                if (Model.CharacterType == CharacterType.Enemy)
                {
                    // 너무 멀다면 풀로 되돌린다.
                    OnDeactivate(false);
                    return;
                }

                distanceToTarget = DistanceToTarget.VeryFar;
                updateInterval = 1.5f;
            }
            else if (distanceSqr >= (farDistance * farDistance))
            {
                updateInterval = 1f;
                distanceToTarget = DistanceToTarget.Far;
            }
            else if (distanceSqr >= (cameraVisibleDistance * cameraVisibleDistance))
            {
                updateInterval = 0.5f;
                distanceToTarget = DistanceToTarget.Nearby;
            }
            else
            {
                updateInterval = 0f;
            }
        }
        else
        {
            Model.SetDistanceToTargetSqr(float.MaxValue);
            updateInterval = 0f;
        }

        Model.SetDistanceToTarget(distanceToTarget);
    }

    protected void ProcessCurrentState()
    {
        if (Model == null)
            return;

        CurrentState.OnStateAction(Model);
    }

    protected void CheckNextState()
    {
        if (!Model.IsDead)
        {
            updateTimer += Time.deltaTime;

            if (updateTimer < updateInterval)
                return;

            // 상태 로직이 무겁기 때문에 업데이트 주기를 조절
            updateTimer = 0;
        }
        
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

    protected void UpdatePhysics()
    {
        if (Model.IsEnablePhysics)
        {
            
        }
        else
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

        if (Model.CharacterSetUpType == CharacterSetUpType.Battle)
            InitializeBattleModel();
    }

    private void InitializeBattleModel()
    {
        // 배틀 이벤트 처리
        battleEventProcessor.Initialize(Model);
        Model.SetEventProcessor(battleEventProcessor);

        // 어빌리티 처리
        abilityProcessor.Initialize(Model);

        // 무기 슬롯 어빌리티 추가
        if (Model.CharacterType == CharacterType.Main)
        {
            var equipmentWeapon = Model.GetEquipment(EquipmentType.Weapon);

            if (equipmentWeapon != null)
                abilityProcessor.AddAbility((int)equipmentWeapon.Ability);

#if CHEAT
            if (CheatManager.CheatConfig.cheatAbility != null)
            {
                foreach (var ability in CheatManager.CheatConfig.cheatAbility)
                {
                    abilityProcessor.AddAbility((int)ability);
                    Logger.Log($"Cheat Ability Added : {ability}");
                }
            }
#endif
        }
        else
        {
            // 주무기 슬롯 어빌리티 추가
            var weaponAbilityId = Model.GetAbilityDataIdBySlot(AbilitySlotType.Weapon);
            if (weaponAbilityId != 0)
                abilityProcessor.AddAbility(weaponAbilityId);

            // 액티브 슬롯 어빌리티 추가
            var activeAbilityId = Model.GetAbilityDataIdBySlot(AbilitySlotType.Active);
            if (activeAbilityId != 0)
                abilityProcessor.AddAbility(activeAbilityId);

            // 패시브 슬롯 어빌리티 추가
            var passiveAbilityId = Model.GetAbilityDataIdBySlot(AbilitySlotType.Passive);
            if (passiveAbilityId != 0)
                abilityProcessor.AddAbility(passiveAbilityId);
        }

        Model.SetAbilityProcessor(abilityProcessor);
    }

    private CharacterActionHandler CreateActionHandler()
    {
        var pathFinder = CreatePathFinder();

        if (actionHandler == null)
            actionHandler = new CharacterActionHandler(animator, rigidBody2D, bodySprite, gameObject, pathFinder);
        
        actionHandler.SetOnFlipX(OnFlipX);
        actionHandler.SetOnStopStateUpdate(OnStateUpdateChange);
        actionHandler.SetOnDeactivate(() => OnDeactivate(true));

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

        return (int)animState;
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
        if (BattleSceneManager.Instance != null)
            BattleSceneManager.Instance.RemoveLiveCharacter(gameObject.GetInstanceID());

        await UniTask.WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

        bodySprite.DeactiveWithFade(1, gameObject);

        BattleSystemManager.Instance.OnDeadCharacter(Model);
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
