using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableCharacterStates/Character/AbilityReady")]
public class CharacterAbilityReadyState : ScriptableCharacterState
{
    public override int Priority => 2;
    public override CharacterAnimState AnimState => CharacterAnimState.Ready;

    public override bool CheckEnterCondition(CharacterUnitModel model)
    {
        model.ReduceReadyCoolTime();

        return model.ReadyCoolTime <= 0;
    }

    public override bool CheckExitCondition(CharacterUnitModel model)
    {
        if (Time.time - model.StateEnterTime < FloatDefine.DEFAULT_MINIMUM_STATE_DURATION)
            return false;

        return model.CurrentAnimState != AnimState;
    }

    public override void OnEnterState(CharacterUnitModel model)
    {
        model.ActionHandler.OnStopPathFind();
        model.SetIsEnablePhysics(false);
    }

    public override void OnExitState(CharacterUnitModel model)
    {
        model.SetReadyCoolTime(FloatDefine.DEFAULT_ABILITY_READY_COOLTIME);
        model.SetIsEnablePhysics(true);
    }

    public override void OnStateAction(CharacterUnitModel model)
    {
    }
}