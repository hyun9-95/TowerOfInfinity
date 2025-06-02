public class PlayerInfo
{
    public InputWrapper InputWrapper { get; private set; }

    public BattleEventTriggerModel[] SkillInfos { get; private set; } = new BattleEventTriggerModel[3];

    public bool IsEnableAttack { get; private set; }

    public void SetPlayerCharacterInputWrapper(InputWrapper wrapper)
    {
        InputWrapper = wrapper;
    }

    public void SetSkillInfos(BattleEventTriggerModel[] skillInfos)
    {
        SkillInfos = skillInfos;
    }

    public void SetSkillInfo(BattleEventTriggerModel skillInfo, int index)
    {
        SkillInfos[index] = skillInfo;
    }

    public void SetEnableAttack(bool value)
    {
        IsEnableAttack = value;
    }

    public BattleEventTriggerModel GetSkillInfoByInput()
    {
        if (InputWrapper.PlayerInput == PlayerInput.None)
            return null;

        switch (InputWrapper.PlayerInput)
        {
            case PlayerInput.SkillSlot_1:
                return SkillInfos[0];

            case PlayerInput.SkillSlot_2:
                return SkillInfos[1];

            case PlayerInput.SkillSlot_3:
                return SkillInfos[2];

            default:
                return null;
        }
    }
}
