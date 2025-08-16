using System;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableCharacterStates/Character/Ability")]
public class CharacterAbilityState : SoloScriptableState, IObserver
{
    [SerializeField]
    private AbilityDefine abilityDefine;

    [SerializeField, Range(0f, 1f)]
    private float probability;

    [SerializeField]
    private CharacterAnimState animState;

    [SerializeField]
    private int targetProbability;

    private Ability ability;
    private bool isPicked;
    private int currentPriority;

    public override int Priority => currentPriority;
    public override CharacterAnimState AnimState => animState;

    private void OnEnable()
    {
        ObserverManager.AddObserver(BattleObserverID.AbilityReady, this);
    }

    private void OnDisable()
    {
        ability = null;
        ObserverManager.RemoveObserver(BattleObserverID.AbilityReady, this);
    }

    protected override bool CheckEnterConditionInternal(CharacterUnitModel model)
    {
        return isPicked && ability != null && ability.IsCastable;
    }

    protected override bool CheckExitConditionInternal(CharacterUnitModel model)
    {
        if (model.CurrentAnimState != animState)
            return true;

        if (ability != null && !ability.IsCastable)
            return true;

        return false;
    }

    protected override void OnEnterStateInternal(CharacterUnitModel model)
    {
        if (ability == null)
            return;

        if (ability.IsCastable == true)
            ability.Cast();
    }

    protected override void OnExitStateInternal(CharacterUnitModel model)
    {
        if (ability != null)
            ability.SetRemainCoolTime(ability.Model.CoolTime);

        currentPriority = 0;
        isPicked = false;
    }

    private void CreateAbility(CharacterUnitModel owner)
    {
        int abilityId = (int)abilityDefine;
        ability = AbilityFactory.Create<Ability>(abilityId, owner);
    }

    void IObserver.HandleMessage(Enum observerMessage, IObserverParam observerParam)
    {
        if (!observerMessage.Equals(BattleObserverID.AbilityReady))
            return;

        if (observerParam is not BattleObserverParam param || param.ModelValue == null)
            return;

        var model = param.ModelValue;

        if (GetOwner() != model)
            return;
        
        if (ability == null)
            CreateAbility(model);

        if (ability.IsCastable)
        {
            float randomValue = UnityEngine.Random.Range(0f, 1f);
            isPicked = randomValue <= probability;

            if (isPicked)
                currentPriority = targetProbability;
        }
    }
}