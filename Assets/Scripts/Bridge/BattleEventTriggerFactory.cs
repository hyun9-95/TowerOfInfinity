public static class BattleEventTriggerFactory
{
    public static BattleEventTrigger Create(BattleEventTriggerType type)
    {
        BattleEventTrigger newTrigger = type switch
        {
            BattleEventTriggerType.Projectile => new ProjectileBattleEventTrigger(),
            BattleEventTriggerType.FollowCollider => new FollowColliderBattleEventTrigger(),
            BattleEventTriggerType.InRange => new InRangeTargetBattleEventTrigger(),
            BattleEventTriggerType.InRangeFollow => new InRangeFollowBattleEventTrigger(),
            // BattleEventTriggerType.Movement => new MovementBattleEventTrigger(),
            _ => null
        };

        return newTrigger;
    }
}
