using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Character/State/Enemy/Ready")]
public class CharacterReadyState : ScriptableCharacterState
{
    public override int Priority => 90;
    public override CharacterAnimState AnimState => CharacterAnimState.Ready;

    public override bool CheckEnterCondition(CharacterUnitModel model)
    {
        return model.ReadyCoolTime <= 0;
    }

    public override bool CheckExitCondition(CharacterUnitModel model)
    {
        if (IsJustEntered(model))
            return false;

        return model.CurrentAnimState != AnimState;
    }

    public override void OnEnterState(CharacterUnitModel model)
    {
        model.ActionHandler.OnStopPathFind();
    }

    public override void OnExitState(CharacterUnitModel model)
    {
        model.SetReadyCoolTime(FloatDefine.DEFAULT_ABILITY_READY_COOLTIME);
    }

    public override void OnStateAction(CharacterUnitModel model)
    {
    }
}