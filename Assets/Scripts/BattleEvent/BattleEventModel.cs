public class BattleEventModel
{
    public CharacterUnitModel Sender { get; private set; }
    public CharacterUnitModel Receiver { get; private set; }
    public int DataID { get; private set; }
    public BattleEventType BattleEventType { get; private set; }
    public BattleEventCategory EffectCategoryType { get; private set; }
    public BattleEventGroup EffectGroupType { get; private set; }
    public StatType AffectStat { get; private set; }
    public StatReference StatReference { get; private set; }
    public StatReferenceCondition StatReferenceCondition { get; private set; }
    public StatusDirection StatusDirection { get; private set; }
    public bool Stackable { get; private set; }
    public float Duration { get; private set; }
    public float Value { get; private set; }
    public float ApplyIntervalSeconds { get; private set; }

    public void Reset()
    {
        DataID = 0;
        BattleEventType = default;
        Sender = null;
        Receiver = null;
        Value = 0f;
    }

    public void SetDataId(int skillDataId)
    {
        DataID = skillDataId;
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

    public void SetAffectStat(StatType statType)
    {
        AffectStat = statType;
    }

    public void SetDuration(float duration)
    {
        Duration = duration;
    }

    public void SetValue(float value)
    {
        Value = value;
    }
}
