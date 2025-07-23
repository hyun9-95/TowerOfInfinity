public struct BattleEventFactory
{
    public BattleEvent CreateNewBattleEvent(BattleEventType eventType)
    {
        switch (eventType)
        {
            case BattleEventType.Damage:
                return new DamageBattleEvent();

            default:
                Logger.Error($"정의되지 않은 BattleStatusEffect : {eventType}");
                break;
        }

        return null;
    }
}
