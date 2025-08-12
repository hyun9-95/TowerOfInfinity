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
    private bool debugLog;

    private ScriptableCharacterStateGroup stateGroup;
    private ScriptableCharacterModuleGroup moduleGroup;
    private ScriptableCharacterState defaultState;
    private bool activated = false;

    private AbilityProcessor abilityProcessor = new();
    private BattleEventProcessor battleEventProcessor = new();
    private CharacterActionHandler actionHandler;

    private Dictionary<ModuleType, IModuleModel> moduleModelDic;

    private Dictionary<int, CharacterAnimState> shortNameHashDic = new();

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

        // 우선순위가 높은 순으로 정렬하므로
        // 가장 마지막에 있는 것이 기본 상태가 된다.
        stateGroup.Sort();
        defaultState = stateGroup.StateList.Last();
        ChangeState(defaultState);

        activated = true;
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

        characterCollider2D.enabled = false;
        triggerCollider2D.enabled = false;

        moduleGroup.OnEventCharacterDeactivate(Model, moduleModelDic);

        abilityProcessor.Cancel();
        battleEventProcessor.Cancel();
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

        if (Model.CharacterSetUpType == CharacterSetUpType.Battle)
        {
            abilityProcessor.Update();
            battleEventProcessor.Update();
        }

        // 활성 상태 동기화
        Model.SetIsActivate(activated);
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
            Model.SetCurrentAnimState(animState);
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

        var resolvedAnimState = ResolveAnimStateParameter(state.AnimState);
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

        if (Model.CharacterSetUpType == CharacterSetUpType.Battle)
            InitializeBattleModel();
    }

    private void InitializeBattleModel()
    {
        // 배틀 이벤트 처리
        battleEventProcessor.SetOwner(Model);
        Model.SetEventProcessor(battleEventProcessor);

        // 어빌리티 처리
        abilityProcessor.SetOwner(Model);

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
