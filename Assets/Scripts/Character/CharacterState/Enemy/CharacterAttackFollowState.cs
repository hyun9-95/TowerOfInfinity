using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Character/State/Enemy/Attack Follow")]
public class CharacterAttackFollowState : ScriptableCharacterState
{
    public override int Priority => 1;
    public override CharacterAnimState AnimState => CharacterAnimState.Move;

    public override bool CheckEnterCondition(CharacterUnitModel model)
    {
        if (model.Target == null && model.Target.IsDead)
            return false;

        if (IsInAttackRange(model))
            return false;

        return true;
    }

    public override bool CheckExitCondition(CharacterUnitModel model)
    {
        if (model.Target == null || model.Target.IsDead)
            return true;

        if (IsJustEntered(model))
            return false;

        return IsInAttackRange(model);
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
        // 먼저 거리 확인
        var attackRange = model.AbilityProcessor.GetPrimaryWeaponRange();
        bool isInRange = model.DistanceToTargetSqr <= attackRange * attackRange;
        

        if (isInRange)
        {
            // 거리 내에 있다면 Y축 차이 확인
            float yDiff = Mathf.Abs(model.Target.Transform.position.y - model.Transform.position.y);
            
            if (yDiff >= 1f)
            {
                // Y축 차이가 크면 Y축만 맞추는 이동
                Vector3 targetPosition = model.Target.Transform.position;
                Vector3 currentPosition = model.Transform.position;
                
                Vector2 yDirection = new Vector2(0, targetPosition.y > currentPosition.y ? 1 : -1);
                Vector2 direction = (model.Target.Transform.position - model.Transform.position).normalized;

                model.ActionHandler.OnMovement(yDirection, model.GetStatValue(StatType.MoveSpeed), true);
                model.ActionHandler.Flip(direction);
                return;
            }
        }
        
        // 일반 추적
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