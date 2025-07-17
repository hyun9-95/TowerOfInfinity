public class BattleEventModel
{
    public CharacterUnitModel Sender { get; private set; }
    public CharacterUnitModel Receiver { get; private set; }
    public int DataID { get; private set; }
    public BattleEventType EventType { get; private set; }
    public BattleEventCategory Category { get; private set; }
    public BattleEventGroup Group { get; private set; }
    public StatType AffectStat { get; private set; }
    public StatReference StatReference { get; private set; }
    public StatReferenceCondition StatReferenceCondition { get; private set; }
    public StatusDirection StatusDirection { get; private set; }
    public bool Stackable { get; private set; }
    public float Duration { get; private set; }
    public float Value { get; private set; }
    public float ApplyIntervalSeconds { get; private set; }

    public void Initialize(CharacterUnitModel sender, CharacterUnitModel receiver, DataBattleEvent dataBattleEvent, int level)
    {
        Sender = sender;
        Receiver = receiver;
        DataID = dataBattleEvent.Id;
        EventType = dataBattleEvent.Type;
        Category = dataBattleEvent.BattleEventCategory;
        Group = dataBattleEvent.BattleEventGroup;
        AffectStat = dataBattleEvent.AffectStat;
        StatReference = dataBattleEvent.StatReference;
        StatReferenceCondition = dataBattleEvent.StatReferenceCondition;
        StatusDirection = dataBattleEvent.StatusDirection;
        Stackable = dataBattleEvent.Stackable;
        Duration = dataBattleEvent.Duration[level];
        Value = dataBattleEvent.Value[level];
        ApplyIntervalSeconds = dataBattleEvent.ApplyIntervalSeconds[level];
    }
}
