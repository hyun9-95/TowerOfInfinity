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
        if (model.InputWrapper == null)
            return false;

        return !model.ActionHandler.IsRolling && model.InputWrapper.PlayerInput == PlayerInput.Roll;
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
        await model.ActionHandler.OnRollingAsync(rollDelay, model.InputWrapper.Movement.normalized,
            model.GetStatValue(StatType.MoveSpeed) * speedMultiplier);

        model.AbilityProcessor?.Cast(CastingType.OnRoll);
    }
}