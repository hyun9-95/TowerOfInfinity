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
        BattleSystemManager.Instance.OnHitEffect
            (Model.Receiver, DamageType.Poison);
    }

    public override void OnInterval()
    {
        BattleSystemManager.Instance.
            OnDamage(Model.Sender, Model.Receiver, GetAppliableStatValue(), DamageType.Poison);
    }
	#endregion
}
