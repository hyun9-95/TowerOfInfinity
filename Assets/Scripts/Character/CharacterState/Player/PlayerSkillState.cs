using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableCharacterStates/Player/Skill")]
public class PlayerSkillState : ScriptableCharacterState
{
    public override int Priority => 9;
    public override CharacterAnimState AnimState => animState;
    private CharacterAnimState animState;

    public override bool CheckEnterCondition(CharacterUnitModel model)
    {
        return model.InputWrapper.IsSkillInput;
    }

    public override bool CheckExitCondition(CharacterUnitModel model)
    {
        return model.CurrentAnimState == CharacterAnimState.Idle ||
            model.CurrentAnimState != animState;
    }

    public override void OnEnterState(CharacterUnitModel model)
    {
        if (model.InputWrapper.IsSkillInput)
        {
            BattleEventTriggerModel skillInfo = null;

            if (skillInfo != null)
            {
                animState = CharacterAnimState.Slash;
                skillInfo.SetSender(model);

                BattleEventTrigger battleSkillTrigger = BattleEventTriggerFactory.Create(skillInfo.TriggerType);
                battleSkillTrigger.SetModel(skillInfo);
                battleSkillTrigger.Process().Forget();
            }
        }
    }

    public override void OnStateAction(CharacterUnitModel model) { }
}