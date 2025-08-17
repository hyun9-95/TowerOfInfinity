public class DamageBattleEvent : BattleEvent
{
    public override void OnStart()
    {
        BattleSystemManager.OnDamage(
            Model.Sender, Model.Receiver, GetAppliableStatValue(), DamageType.Normal);
    }
}
