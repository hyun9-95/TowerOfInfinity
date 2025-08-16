using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableCharacterStates/Player/Attack")]
public class PlayerAttackState : ScriptableCharacterState
{
    public override int Priority => 10;

    public override CharacterAnimState AnimState => CharacterAnimState.Attack;

    public override bool CheckEnterCondition(CharacterUnitModel model)
    {
        if (InputManager.InputInfo.ActionInput != ActionInput.Attack)
            return false;

        if (model.AbilityProcessor == null)
            return false;

        if (!model.AbilityProcessor.IsPrimaryWeaponSlotReady())
            return false;

        return !model.IsAttackState();
    }

    public override bool CheckExitCondition(CharacterUnitModel model)
    {
        if (model.AbilityProcessor == null)
            return true;

        return !model.AbilityProcessor.IsPrimaryWeaponSlotReady() && !model.IsAttackState();
    }

    public override void OnStateAction(CharacterUnitModel model)
    {
        if (model.AbilityProcessor == null)
            return;

        if (InputManager.InputInfo.IsMove)
        {
            model.ActionHandler.OnMovement(InputManager.InputInfo.Movement,
            model.GetStatValue(StatType.MoveSpeed), true);
        }

        if (model.AbilityProcessor.IsPrimaryWeaponSlotReady() && model.IsAttackState())
            model.AbilityProcessor.CastPrimaryWeapon(model.GetAnimationDelay((AnimState))).Forget();
    }
}
