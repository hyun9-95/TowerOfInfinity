using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableCharacterStates/Character/Attack")]
public class CharacterAttackState : ScriptableCharacterState
{
    public override int Priority => 4;
    public override CharacterAnimState AnimState => CharacterAnimState.Attack;

    public override bool CheckEnterCondition(CharacterUnitModel model)
    {
        if (model.AbilityProcessor == null)
            return false;

        return model.AbilityProcessor.IsPrimaryWeaponSlotReady() && IsInAttackRange(model) && !model.IsAttackState();
    }

    public override bool CheckExitCondition(CharacterUnitModel model)
    {
        if (model.AbilityProcessor == null)
            return true;

        return !model.AbilityProcessor.IsPrimaryWeaponSlotReady() && !model.IsAttackState();
    }

    public override void OnEnterState(CharacterUnitModel model)
    {
        model.ActionHandler.OnStopPathFind();
    }

    public override void OnStateAction(CharacterUnitModel model)
    {
        if (model.AbilityProcessor == null)
            return;

        if (model.AbilityProcessor.IsPrimaryWeaponSlotReady() && model.IsAttackState())
            model.AbilityProcessor.CastPrimaryWeapon(GetAnimationDelay(AnimState)).Forget();
    }

    private bool IsInAttackRange(CharacterUnitModel model)
    {
        if (model.Agent == null || model.Target == null)
            return false;

        var attackRange = model.AbilityProcessor.GetPrimaryWeaponRange();

        var distance = Vector3.Distance(model.Transform.position, model.Target.Transform.position);
        return distance <= attackRange;
    }
}