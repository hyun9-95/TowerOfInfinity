using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableCharacterStates/Character/Follow")]
public class CharacterFollowState : ScriptableCharacterState
{
    public override int Priority => 1;
    public override CharacterAnimState AnimState => CharacterAnimState.Move;

    public override bool CheckEnterCondition(CharacterUnitModel model)
    {
        return model.Target != null && !model.Target.IsDead && !IsStoppingDistance(model);
    }

    public override bool CheckExitCondition(CharacterUnitModel model)
    {
        return model.Target == null || IsStoppingDistance(model) || model.Target.IsDead;
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
    }

    public override void OnStateAction(CharacterUnitModel model)
    {
        if (model.PathFindType == PathFindType.AStar)
        {
            model.ActionHandler.OnAStarMoveAlongPath();
            model.SetRepathTimer(model.RepathTimer + Time.deltaTime);

            if (model.RepathTimer >= FloatDefine.ASTAR_REPATH_COOLTIME)
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

        var distance = Vector3.Distance(model.Transform.position, model.Target.Transform.position);
        return distance <= model.Agent.stoppingDistance;
    }
}