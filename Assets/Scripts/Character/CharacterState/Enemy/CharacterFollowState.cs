using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Character/State/Enemy/Follow")]
public class CharacterFollowState : ScriptableCharacterState
{
    public override int Priority => 1;
    public override CharacterAnimState AnimState => CharacterAnimState.Move;

    public override bool CheckEnterCondition(CharacterUnitModel model)
    {
        if (model.Target == null || model.Target.IsDead)
            return false;

        if (IsTooClose(model))
            return false;

        return !IsStoppingDistance(model);
    }

    public override bool CheckExitCondition(CharacterUnitModel model)
    {
        if (model.Target == null || model.Target.IsDead)
            return true;

        if (IsJustEntered(model))
            return false;

        return IsStoppingDistance(model);
    }

    public override void OnEnterState(CharacterUnitModel model)
    {
        if (model.PathFindType == PathFindType.AStar)
        {
            model.ActionHandler.OnAStarUpdatePath(model.Target.Transform.position);
            model.SetRepathTimer(0);
        }
        else
        {
            model.ActionHandler.OnNavmeshPathFind(model.Target.Transform.position);
        }

        Vector2 direction = (model.Target.Transform.position - model.Transform.position).normalized;
        model.ActionHandler.OnMovement(direction, model.GetStatValue(StatType.MoveSpeed), true);
    }

    public override void OnStateAction(CharacterUnitModel model)
    {
        if (model.PathFindType == PathFindType.AStar)
        {
            // 너무 멀다면 직선 이동
            if (model.DistanceToTarget >= DistanceToTarget.Far)
            {
                Vector2 direction = (model.Target.Transform.position - model.Transform.position).normalized;
                model.ActionHandler.OnMovement(direction, model.GetStatValue(StatType.MoveSpeed), true);
                return;
            }

            model.ActionHandler.OnAStarMoveAlongPath();
            model.SetRepathTimer(model.RepathTimer + Time.deltaTime);

            if (model.RepathTimer >= model.GetRepathCoolTime())
            {
                model.SetRepathTimer(0);
                model.ActionHandler.OnAStarUpdatePath(model.Target.Transform.position);
            }
        }
        else
        {
            model.ActionHandler.OnNavmeshPathFind(model.Target.Transform.position);
        }
    }

    public override void OnExitState(CharacterUnitModel model)
    {
        model.ActionHandler.OnStopPathFind();
    }

    private bool IsStoppingDistance(CharacterUnitModel model)
    {
        if (model.Agent == null)
            return false;

        var stoppingDistance = model.Agent.stoppingDistance * model.Agent.stoppingDistance;
        return model.DistanceToTargetSqr <= stoppingDistance;
    }

    // 너무 바로 따라가지 않게
    private bool IsTooClose(CharacterUnitModel model)
    {
        if (model.Agent == null)
            return false;

        var chaseDistance = model.Agent.stoppingDistance * model.Agent.stoppingDistance;
        chaseDistance -= 0.2f;

        return model.DistanceToTargetSqr <= chaseDistance;
    }
}