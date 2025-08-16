using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableCharacterStates/Character/Ability")]
public class CharacterAbilityState : SoloScriptableState
{
    public override int Priority => targetProbability;
    public override CharacterAnimState AnimState => animState;

    [SerializeField]
    private AbilityDefine abilityDefine;

    [SerializeField]
    private CharacterAnimState animState;

    [SerializeField]
    private float animDelay;

    [SerializeField]
    private int targetProbability;

    private Ability ability;
    private float lastEnterTime = 0;

    private void OnDisable()
    {
        ability = null;
    }

    protected override void OnSetOwner(CharacterUnitModel model)
    {
        CreateAbility(model);
    }

    protected override bool CheckEnterConditionInternal(CharacterUnitModel model)
    {
        if (model.CurrentAnimState == CharacterAnimState.Ready)
            return false;

        if (Time.time - lastEnterTime < ability.Model.CoolTime)
            return false;

        return true;
    }

    protected override bool CheckExitConditionInternal(CharacterUnitModel model)
    {
        if (Time.time - model.StateEnterTime < FloatDefine.DEFAULT_MINIMUM_STATE_DURATION)
            return false;

        return model.CurrentAnimState != AnimState;
    }

    protected override void OnEnterStateInternal(CharacterUnitModel model)
    {
        if (ability == null)
            return;

        model.ActionHandler.OnStopPathFind();
        ability.DelayCast(animDelay).Forget();
        lastEnterTime = Time.time;
    }

    private void CreateAbility(CharacterUnitModel owner)
    {
        int abilityId = (int)abilityDefine;
        ability = AbilityFactory.Create<Ability>(abilityId, owner);
    }
}