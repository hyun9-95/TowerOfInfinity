using UnityEngine;

public class BuffBattleEvent : BattleEvent
{
    #region Property
    #endregion

    #region Value
    private float appliedValue = 0;
    #endregion

    #region Function
    public override void OnStart()
    {
        appliedValue = GetAppliableStatValue();

        Model.Receiver.AddStatValue(Model.AffectStat, appliedValue);

        Logger.BattleLog($"BuffStart => {Model.AffectStat} + {appliedValue}");
    }

    public override void OnEnd()
    {
        Model.Receiver.AddStatValue(Model.AffectStat, -appliedValue);
    }
	#endregion
}
