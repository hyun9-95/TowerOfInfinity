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

        if (Time.time - model.StateEnterTime < FloatDefine.DEFAULT_MINIMUM_STATE_DURATION)
            return false;

        return !model.IsAttackState();
    }

    public override void OnEnterState(CharacterUnitModel model)
    {
        model.ActionHandler.OnStopPathFind();

        Vector2 direction = (model.Target.Transform.position - model.Transform.position).normalized;
        model.ActionHandler.Flip(direction);

#if CHEAT
        CheatManager.DrawWireSphereAtPosition(model.Transform.position, model.AbilityProcessor.GetPrimaryWeaponRange());
#endif
    }

    public override void OnStateAction(CharacterUnitModel model)
    {
        if (model.AbilityProcessor == null)
            return;

        if (model.AbilityProcessor.IsPrimaryWeaponSlotReady() && model.IsAttackState())
            model.AbilityProcessor.CastPrimaryWeapon(model.GetAnimationDelay(AnimState)).Forget();
    }

    private bool IsInAttackRange(CharacterUnitModel model)
    {
        if (model.Agent == null || model.Target == null)
            return false;

        var attackRange = model.AbilityProcessor.GetPrimaryWeaponRange();

        bool isClose = model.DistanceToTargetSqr <= attackRange * attackRange;

        if (!isClose)
            return false;

        float yDiff = Mathf.Abs(model.Target.Transform.position.y - model.Transform.position.y);

        return yDiff < 1f;
    }
}