using System.Collections.Generic;

public static class BattleEventTriggerFactory
{
    private static Stack<BattleEventTrigger> triggerPool = new Stack<BattleEventTrigger>();
    private static Stack<BattleEventTriggerModel> triggerModelPool = new Stack<BattleEventTriggerModel>();

    public static BattleEventTrigger Create(BattleEventTriggerType type)
    {
        if (triggerPool.Count > 0)
        {
            var reused = triggerPool.Pop();
            return reused;
        }

        BattleEventTrigger newTrigger = type switch
        {
            BattleEventTriggerType.Projectile => new ProjectileBattleEventTrigger(),
            BattleEventTriggerType.Range => new RangeBattleEventTrigger(),
            BattleEventTriggerType.Collider => new ColliderBattleEventTrigger(),
            // BattleEventTriggerType.Movement => new MovementBattleEventTrigger(),
            _ => null
        };

        return newTrigger;
    }

    public static BattleEventTriggerModel CreateTriggerModel()
    {
        if (triggerModelPool.Count > 0)
            return triggerModelPool.Pop();

        return new BattleEventTriggerModel();
    }

    public static void ReturnToPool(BattleEventTrigger usedTrigger)
    {
        if (usedTrigger == null)
            return;

        usedTrigger.Reset();
        triggerPool.Push(usedTrigger);
    }

    public static void ReturnToPool(BattleEventTriggerModel usedModel)
    {
        if (usedModel == null)
            return;

        usedModel.Reset();
        triggerModelPool.Push(usedModel);
    }
}
