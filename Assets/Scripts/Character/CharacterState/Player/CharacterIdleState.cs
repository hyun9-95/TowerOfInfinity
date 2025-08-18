using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Character/State/Character/Idle")]
public class CharacterIdleState : ScriptableCharacterState
{
    public override int Priority => 0;

    public override CharacterAnimState AnimState => CharacterAnimState.Idle;

    public override void OnStateAction(CharacterUnitModel model)
    {
    }

    public override void OnEnterState(CharacterUnitModel model)
    {
        if (model.Target != null)
        {
            var direction = (model.Target.Transform.position - model.Transform.position).normalized;
            model.ActionHandler.Flip(direction);
        }
    }

    public override bool CheckEnterCondition(CharacterUnitModel model)
    {
        return true;
    }
}
