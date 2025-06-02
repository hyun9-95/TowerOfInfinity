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

    public override void OnStateAction(CharacterUnitModel model)
    {
        model.ActionHandler.OnPathFind(model.Target.Transform.position);
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