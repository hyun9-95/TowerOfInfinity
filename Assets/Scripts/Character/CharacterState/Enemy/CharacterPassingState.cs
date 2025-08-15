using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableCharacterStates/Character/Passing")]
public class CharacterPassingState : ScriptableCharacterState
{
    [SerializeField]
    private float passingTransitionDistance = 3.0f;

    [SerializeField]
    private float passingExitDistance = 8.0f;

    [SerializeField]
    private float passingMoveSpeedMultiplier = 1.5f;

    public override int Priority => 3;
    public override CharacterAnimState AnimState => CharacterAnimState.Move;

    // 캐릭터별 고정 방향 저장
    private static Dictionary<CharacterUnitModel, Vector3> fixedDirections = new Dictionary<CharacterUnitModel, Vector3>();

    public override bool CheckEnterCondition(CharacterUnitModel model)
    {
        return model.Target != null && !model.Target.IsDead && IsPassingDistance(model);
    }

    public override bool CheckExitCondition(CharacterUnitModel model)
    {
        return model.Target == null || model.Target.IsDead || IsExitDistance(model);
    }

    public override void OnEnterState(CharacterUnitModel model)
    {
        model.ActionHandler.OnStopPathFind();

        // 현재 위치에서 타겟 위치까지의 방향 계산 및 저장
        if (model.Target != null)
        {
            Vector3 direction = (model.Target.Transform.position - model.Transform.position).normalized;
            fixedDirections[model] = direction;
        }
    }

    public override void OnStateAction(CharacterUnitModel model)
    {
        // 고정된 방향 계속 이동
        if (fixedDirections.ContainsKey(model))
        {
            Vector3 direction = fixedDirections[model];

            var moveSpeed = model.GetStatValue(StatType.MoveSpeed);

            // 패싱 중에는 속도가 증가함.
            moveSpeed *= passingMoveSpeedMultiplier;

            model.ActionHandler.OnMovement(direction, moveSpeed, true);
        }
    }

    public override void OnExitState(CharacterUnitModel model)
    {
        if (fixedDirections.ContainsKey(model))
            fixedDirections.Remove(model);
    }

    private bool IsPassingDistance(CharacterUnitModel model)
    {
        if (model.Target == null)
            return false;

        return model.DistanceToTargetSqr <= passingTransitionDistance * passingTransitionDistance;
    }

    private bool IsExitDistance(CharacterUnitModel model)
    {
        if (model.Target == null)
            return true;

        return model.DistanceToTargetSqr >= passingExitDistance * passingExitDistance;
    }
}