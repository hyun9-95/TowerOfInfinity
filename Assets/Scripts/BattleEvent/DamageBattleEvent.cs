using UnityEngine;

public class DamageBattleEvent : BattleEvent
{
    public override void OnStart()
    {
        var damage = Formula.GetNormalDamage(Model.Sender, Model.Receiver, Model.Value);
        Model.Receiver.ReduceHp(damage);

        Logger.BattleLog($"{Model.Sender.CharacterDefine} -> {Model.Receiver.CharacterDefine} : {damage} / Remain HP : {Model.Receiver.Hp}");

        // 적인 경우에만 대미지를 표시.
        if (Model.Receiver.TeamTag == TeamTag.Enemy)
        {
            BattleFXManager.Instance.ShowDamage(
                DamageType.Normal,
                Model.Receiver.Transform,
                damage.ToString()
            );
        }
    }
}
