using System.Collections.Generic;

public static class BattleEventFactory
{
    private static Stack<BattleEvent> battleEventStack = new Stack<BattleEvent>();
    private static Stack<BattleEventModel> battleEventModels = new Stack<BattleEventModel>();
    //private static Dictionary<BattleEventDefine, >

    public static BattleEvent Create(BattleEventType type)
    {
        if (battleEventStack.Count > 0)
            return battleEventStack.Pop();

        var newEvent = type switch
        {
            BattleEventType.Damage => new DamageBattleEvent(),
            _ => null
        };

        return newEvent;
    }

    public static BattleEventModel CreateBattleEventModel(BattleEventType type)
    {
        if (battleEventModels.Count > 0)
            return battleEventModels.Pop();

        var newModel = type switch
        {
            _ => new BattleEventModel()
        };

        return newModel;
    }

    public static void ReturnToPool(BattleEvent usedEvent)
    {
        usedEvent.Reset();

        battleEventStack.Push(usedEvent);
    }

    public static void ReturnToPool(BattleEventModel usedModel)
    {
        usedModel.Reset();

        battleEventModels.Push(usedModel);
    }
}
