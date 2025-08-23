public struct BattleEventFactory
{
    public BattleEvent CreateNewBattleEvent(BattleEventType eventType)
    {
        switch (eventType)
        {
            case BattleEventType.Damage:
                return new DamageBattleEvent();

            case BattleEventType.Poison:
                return new PoisonBattleEvent();

            case BattleEventType.Frozen:
                return new FrozenBattleEvent();

            case BattleEventType.Buff:
                return new BuffBattleEvent();

            case BattleEventType.Heal:
                return new HealBattleEvent();

            case BattleEventType.ExpRangeUp:
                return new ExpRangeUpBattleEvent();

            default:
                Logger.Error($"정의되지 않은 BattleEvent : {eventType}");
                break;
        }

        return null;
    }
}
