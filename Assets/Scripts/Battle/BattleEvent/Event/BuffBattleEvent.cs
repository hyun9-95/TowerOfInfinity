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
        AddBuffStat();
    }

    public override void OnReapply()
    {
        AddBuffStat();
    }

    public override void OnEnd()
    {
        Model.Receiver.AddStatValue(Model.AffectStat, -appliedValue);
    }

    private void AddBuffStat()
    {
        appliedValue = GetAppliableStatValue();

        Model.Receiver.AddStatValue(Model.AffectStat, appliedValue);

        Logger.BattleLog($"Buff  => {Model.AffectStat} + {appliedValue}");
    }
	#endregion
}
