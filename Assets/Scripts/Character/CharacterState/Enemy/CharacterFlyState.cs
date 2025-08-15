using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableCharacterStates/Character/Fly")]
public class CharacterFlyState : ScriptableCharacterState
{
    [Header("Fly State Settings")]
    [SerializeField] private float passingTransitionDistance = 3.0f;

    public override int Priority => 1;
    public override CharacterAnimState AnimState => CharacterAnimState.Move;

    public override bool CheckEnterCondition(CharacterUnitModel model)
    {
        return model.Target != null && !model.Target.IsDead && !IsPassingDistance(model);
    }

    public override bool CheckExitCondition(CharacterUnitModel model)
    {
        return model.Target == null || model.Target.IsDead || IsPassingDistance(model);
    }

    public override void OnEnterState(CharacterUnitModel model)
    {
        model.ActionHandler.OnStopPathFind();
    }

    public override void OnStateAction(CharacterUnitModel model)
    {
        if (model.Target == null)
            return;

        var moveSpeed = model.GetStatValue(StatType.MoveSpeed);

        // 타겟 방향으로 직선 이동
        Vector2 direction = (model.Target.Transform.position - model.Transform.position).normalized;
        model.ActionHandler.OnMovement(direction, moveSpeed, true);
    }

    private bool IsPassingDistance(CharacterUnitModel model)
    {
        if (model.Target == null)
            return false;

        var distance = Vector3.Distance(model.Transform.position, model.Target.Transform.position);
        return distance <= passingTransitionDistance;
    }
}