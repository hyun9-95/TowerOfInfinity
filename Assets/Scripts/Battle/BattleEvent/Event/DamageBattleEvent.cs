public class DamageBattleEvent : BattleEvent
{
    public override void OnStart()
    {
        BattleApi.OnDamage(
            Model.Sender, Model.Receiver, GetAppliableStatValue(), DamageType.Normal);
    }
}
