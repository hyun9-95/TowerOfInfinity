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
        return model.InputWrapper.PlayerInput == PlayerInput.Roll;
    }

    public override bool CheckExitCondition(CharacterUnitModel model)
    {
        return model.CurrentAnimState != CharacterAnimState.Roll;
    }

    public override void OnEnterState(CharacterUnitModel model)
    {
        DelayAddForce(model).Forget();
    }

    public override void OnStateAction(CharacterUnitModel model)
    {

    }

    private async UniTask DelayAddForce(CharacterUnitModel pcModel)
    {
        await UniTaskUtils.DelaySeconds(rollDelay, cancellationToken: TokenPool.Get(GetHashCode()));
        pcModel.ActionHandler.OnAddForce(pcModel.InputWrapper.Movement.normalized,
            FloatDefine.DEFAULT_MOVE_SPEED * speedMultiplier);
    }
}