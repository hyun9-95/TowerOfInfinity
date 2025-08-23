using UnityEngine;

public class PoisonBattleEvent : BattleEvent
{
    #region Property
    #endregion

    #region Value
    #endregion

    #region Function
    public override void OnStart()
    {
        BattleApi.OnDamage
            (Model.Sender, Model.Receiver, GetAppliableStatValue(), DamageType.Poison);
    }

    public override void OnInterval()
    {
        BattleApi.OnDamage
            (Model.Sender, Model.Receiver, GetAppliableStatValue(), DamageType.Poison);
    }
	#endregion
}
