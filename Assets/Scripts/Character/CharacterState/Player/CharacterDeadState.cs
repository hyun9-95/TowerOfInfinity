using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Character/State/Character/Dead")]
public class CharacterDeadState : ScriptableCharacterState
{
    public override int Priority => IntDefine.STATE_DEAD_PRIORITY;

    public override CharacterAnimState AnimState => CharacterAnimState.Die;

    public override void OnEnterState(CharacterUnitModel model)
    {
        model.ActionHandler.OnDeactivate();
    }

    public override void OnStateAction(CharacterUnitModel model)
    {
    }

    public override bool CheckEnterCondition(CharacterUnitModel model)
    {
        return model.Hp <= 0;
    }
}
