using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableCharacterStates/Character/Dead")]
public class CharacterDeadState : ScriptableCharacterState
{
    public override int Priority => 100;

    public override CharacterAnimState AnimState => CharacterAnimState.Die;

    public override void OnStateAction(CharacterUnitModel model)
    {
        model.ActionHandler.OnDead();
    }

    public override bool CheckEnterCondition(CharacterUnitModel model)
    {
        return model.Hp <= 0;
    }
}
