using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Character/State/Player/Move")]
public class PlayerMoveState : ScriptableCharacterState
{
    public override int Priority => 1;

    public override CharacterAnimState AnimState => CharacterAnimState.Move;

    public override bool CheckEnterCondition(CharacterUnitModel model)
    {
        return InputManager.InputInfo.IsMove && !model.IsDead;
    }

    public override bool CheckExitCondition(CharacterUnitModel model)
    {
        return !InputManager.InputInfo.IsMove || model.IsDead;
    }

    public override void OnStateAction(CharacterUnitModel model)
    {
        model.ActionHandler.OnMovement(InputManager.InputInfo.Movement,
            model.GetStatValue(StatType.MoveSpeed), true);
    }
}
