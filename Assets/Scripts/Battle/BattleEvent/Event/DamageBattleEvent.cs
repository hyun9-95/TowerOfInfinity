public class DamageBattleEvent : BattleEvent
{
    public override void OnStart()
    {
        BattleSystemManager.Instance.
            OnDamage(Model.Sender, Model.Receiver, GetAppliableStatValue(), DamageType.Normal);
    }
}
