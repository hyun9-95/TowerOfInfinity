using UnityEngine;

public class BattleEventTriggerModel
{
    public DataBattleEvent EventData { get; private set; }

    public BattleEventTriggerType TriggerType { get; private set; }

    public BattleEventTargetType TargetType { get; private set; }

    public CharacterUnitModel Sender { get; private set; }

    public string PrefabName { get; private set; }

    public string HitEffectPrefabName { get; private set; }

    public Vector2 Direction { get; private set; }

    public int TargetCount { get; private set; }

    public float Range { get; private set; }

    public float Speed { get; private set; }

    public float Scale { get; private set; }

    public float Value { get; private set; }

    public BattleEventTriggerModel Clone()
    {
        return MemberwiseClone() as BattleEventTriggerModel;
    }

    public void Reset()
    {
        EventData = default;
        TriggerType = BattleEventTriggerType.None;
        TargetType = BattleEventTargetType.None;
        Direction = Vector2.zero;
        TargetCount = 0;
        Sender = null;
        PrefabName = string.Empty;
        Range = 0f;
        Speed = 0f;
    }

    public void SetSkillInfoByData(DataBattleEvent battleEvent)
    {
        if (battleEvent.IsNull)
            return;

        EventData = battleEvent;
        TriggerType = battleEvent.TriggerType;
        TargetType = battleEvent.TargetType;
        PrefabName = battleEvent.PrefabName;
        HitEffectPrefabName = battleEvent.HitEffectPrefabName;
    }
    
    public void SetBalance(AbilityBalance balance, int level)
    {
        if (balance == null)
            return;

        TargetCount = balance.GetTargetCount(level);
        Range = balance.GetRange(level);
        Speed = balance.GetSpeed(level);
        Value = balance.GetValue(level);
        Scale = balance.GetScale(level);
    }

    public BattleEvent CreateBattleEvent(CharacterUnitModel receiver)
    {
        var battleEventModel = BattleEventFactory.CreateBattleEventModel(EventData.Type);
        battleEventModel.SetRecevier(receiver);
        battleEventModel.SetSender(Sender);
        battleEventModel.SetSkillDataId(EventData.Id);
        battleEventModel.SetBattleEventType(EventData.Type);
        battleEventModel.SetValue(Value);

        var battleEvent = BattleEventFactory.Create(battleEventModel.BattleEventType);
        battleEvent.SetModel(battleEventModel);

        return battleEvent;
    }

    public void SetSender(CharacterUnitModel owner)
    {
        Sender = owner;
    }

    public void SetBattleTargetCount(int value)
    {
        TargetCount = value;
    }

    public void SetDirection(Vector3 value)
    {
        Direction = value;
    }
}
