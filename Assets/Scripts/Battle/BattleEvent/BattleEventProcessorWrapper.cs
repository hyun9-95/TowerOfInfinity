using UnityEngine;

public class BattleEventProcessorWrapper
{
    public BattleEventProcessorWrapper(BattleEventProcessor newProcessor)
    {
        processor = newProcessor;
    }
    #region Property
    #endregion

    #region Value
    private BattleEventProcessor processor;
    #endregion

    #region Function

    public void SendBattleEvent(BattleEventModel model)
    {
        processor.SendBattleEvent(model);
    }
    #endregion
}
