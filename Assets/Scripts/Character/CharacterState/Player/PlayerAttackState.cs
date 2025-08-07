using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableCharacterStates/Player/Attack")]
public class PlayerAttackState : ScriptableCharacterState
{
    public override int Priority => 10;

    public override CharacterAnimState AnimState => CharacterAnimState.Attack;

    public override bool CheckEnterCondition(CharacterUnitModel model)
    {
        if (model.AbilityProcessor == null)
            return false;

        return model.AbilityProcessor.IsPrimaryWeaponSlotReady() && !model.IsAttackState();
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

        if (model.InputWrapper.IsMove)
        {
            model.ActionHandler.OnMovement(model.InputWrapper.Movement,
            model.GetStatValue(StatType.MoveSpeed), true);
        }

        if (model.AbilityProcessor.IsPrimaryWeaponSlotReady() && model.IsAttackState())
            model.AbilityProcessor.CastPrimaryWeapon(GetAnimationDelay(AnimState)).Forget();
    }
}
