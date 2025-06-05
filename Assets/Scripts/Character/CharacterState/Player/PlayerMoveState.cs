using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableCharacterStates/Player/Move")]
public class PlayerMoveState : ScriptableCharacterState
{
    public override int Priority => 1;

    public override CharacterAnimState AnimState => CharacterAnimState.Move;

    public override bool CheckEnterCondition(CharacterUnitModel model)
    {
        return model.InputWrapper.IsMove && !model.IsDead;
    }

    public override bool CheckExitCondition(CharacterUnitModel model)
    {
        return !model.InputWrapper.IsMove ||
            model.InputWrapper.PlayerInput != PlayerInput.None ||
            model.IsDead;
    }

    public override void OnStateAction(CharacterUnitModel model)
    {
        model.ActionHandler.OnMovement(model.InputWrapper.Movement,
            model.GetStatValue(StatType.MoveSpeed), true);
    }
}
