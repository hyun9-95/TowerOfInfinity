using UnityEngine;

public class HealBattleEvent : BattleEvent
{
    #region Property
    #endregion

    #region Value
    #endregion

    #region Function

    public override void OnStart()
    {
        Heal();
    }

    public override void OnInterval()
    {
        Heal();
    }

    private void Heal()
    {
        var value = GetAppliableStatValue();
        BattleApi.OnHeal(Model.Sender, Model.Receiver, value);

        Logger.BattleLog($"Heal => {value}");
    }
	#endregion
}
