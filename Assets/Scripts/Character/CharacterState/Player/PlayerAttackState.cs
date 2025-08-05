using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableCharacterStates/Player/Attack")]
public class PlayerAttackState : ScriptableCharacterState
{
    public override int Priority => 10;

    public override CharacterAnimState AnimState => CharacterAnimState.Attack;

    public override bool CheckEnterCondition(CharacterUnitModel model)
    {
        return model.AbilityProcessor.IsPrimaryWeaponReady() && model.CurrentAnimState != AnimState;
    }

    public override bool CheckExitCondition(CharacterUnitModel model)
    {
        return !model.AbilityProcessor.IsPrimaryWeaponReady() && model.CurrentAnimState != AnimState;
    }

    public override void OnStateAction(CharacterUnitModel model)
    {
        if (model.InputWrapper.IsMove)
        {
            model.ActionHandler.OnMovement(model.InputWrapper.Movement,
            model.GetStatValue(StatType.MoveSpeed), true);
        }

        if (model.AbilityProcessor.IsPrimaryWeaponReady() && model.CurrentAnimState == CharacterAnimState.Attack)
            model.AbilityProcessor.CastPrimaryWeapon(GetAnimationDelay(AnimState)).Forget();
    }
}
