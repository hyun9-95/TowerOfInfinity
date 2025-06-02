public class BattleEventModel
{
    public int SkillDataId { get; private set; }

    public BattleEventType BattleEventType { get; private set; }

    public CharacterUnitModel Sender { get; private set; }

    public CharacterUnitModel Receiver { get; private set; }

    public float Value { get; private set; }

    public void Reset()
    {
        SkillDataId = 0;
        BattleEventType = default;
        Sender = null;
        Receiver = null;
        Value = 0f;
    }

    public void SetSkillDataId(int skillDataId)
    {
        SkillDataId = skillDataId;
    }

    public void SetBattleEventType(BattleEventType processType)
    {
        BattleEventType = processType;
    }

    public void SetSender(CharacterUnitModel sender)
    {
        Sender = sender;
    }

    public void SetRecevier(CharacterUnitModel recevier)
    {
        Receiver = recevier;
    }

    public void SetValue(float value)
    {
        Value = value;
    }
}
