using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Character/State/Player/Skill")]
public class PlayerSkillState : ScriptableCharacterState
{
    public override int Priority => 9;
    public override CharacterAnimState AnimState => animState;
    private CharacterAnimState animState;

    public override bool CheckEnterCondition(CharacterUnitModel model)
    {
        return false;
    }

    public override bool CheckExitCondition(CharacterUnitModel model)
    {
        return true;
    }

    public override void OnEnterState(CharacterUnitModel model)
    {
    }

    public override void OnStateAction(CharacterUnitModel model) { }
}