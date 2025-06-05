using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableCharacterStates/Player/Attack")]
public class PlayerAttackState : ScriptableCharacterState
{
    public override int Priority => 10;

    public override CharacterAnimState AnimState => CharacterAnimState.Attack;

    public override bool CheckEnterCondition(CharacterUnitModel model)
    {
        return model.DefaultWeapon != null && !model.DefaultWeapon.IsProcessing;
    }

    public override bool CheckExitCondition(CharacterUnitModel model)
    {
        return model.DefaultWeapon.IsProcessing && model.CurrentAnimState != AnimState;
    }

    public override void OnStateAction(CharacterUnitModel model)
    {
        if (model.InputWrapper.IsMove)
        {
            model.ActionHandler.OnMovement(model.InputWrapper.Movement,
            model.GetStatValue(StatType.MoveSpeed), true);
        }
        
        if (!model.DefaultWeapon.IsProcessing && model.CurrentAnimState == CharacterAnimState.Attack)
            model.DefaultWeapon.ActivateOneTime().Forget();
    }
}
