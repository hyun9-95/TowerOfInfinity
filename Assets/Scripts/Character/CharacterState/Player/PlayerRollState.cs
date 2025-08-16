using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableCharacterStates/Player/Roll")]
public class PlayerRollState : ScriptableCharacterState
{
    [SerializeField]
    private float rollDelay = 0.4f;

    [SerializeField]
    private float speedMultiplier = 2f;

    public override int Priority => 10;

    public override CharacterAnimState AnimState => CharacterAnimState.Roll;

    public override bool CheckEnterCondition(CharacterUnitModel model)
    {
        if (InputManager.InputInfo == null)
            return false;

        return !model.ActionHandler.IsRolling && InputManager.InputInfo.ActionInput == ActionInput.Roll;
    }

    public override bool CheckExitCondition(CharacterUnitModel model)
    {
        return !model.ActionHandler.IsRolling && model.CurrentAnimState != CharacterAnimState.Roll;
    }

    public override void OnEnterState(CharacterUnitModel model)
    {
        StartRolling(model).Forget();
    }

    public override void OnStateAction(CharacterUnitModel model)
    {
        model.SetIsEnablePhysics(true);
    }

    public override void OnExitState(CharacterUnitModel model)
    {
        model.SetIsEnablePhysics(false);
    }

    private async UniTask StartRolling(CharacterUnitModel model)
    {
        Vector2 rollDirection = GetRollDirection(model);

        model.AbilityProcessor.DelayCast(CastingType.OnRoll, rollDelay);

        await model.ActionHandler.OnRollingAsync(rollDelay, rollDirection,
            model.GetStatValue(StatType.MoveSpeed) * speedMultiplier);
    }

    private Vector2 GetRollDirection(CharacterUnitModel model)
    {
        Vector2 movement = InputManager.InputInfo.Movement;
        
        if (movement == Vector2.zero)
        {
            return model.IsFlipX ? Vector2.left : Vector2.right;
        }
        
        Vector2 adjustedMovement = AdjustRollAngle(movement);
        return adjustedMovement.normalized;
    }

    // 수직으로 구르지 않게 조절
    private Vector2 AdjustRollAngle(Vector2 movement)
    {
        float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
        
        if (Mathf.Abs(angle) > 70f && Mathf.Abs(angle) < 110f)
        {
            float adjustedAngle = angle > 0f ? 70f : -70f;
            return new Vector2(Mathf.Cos(adjustedAngle * Mathf.Deg2Rad), Mathf.Sin(adjustedAngle * Mathf.Deg2Rad));
        }
        
        return movement;
    }
}