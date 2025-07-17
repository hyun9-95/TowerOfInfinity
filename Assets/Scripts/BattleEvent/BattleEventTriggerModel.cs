using UnityEngine;

public class BattleEventTriggerModel
{
    public int AbilityDataId { get; private set; }
    public int Level { get; private set; }
    public DataBattleEvent BattleEventData { get; private set; }

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

    public float Duration { get; private set; }


    public BattleEventTriggerModel Clone()
    {
        return MemberwiseClone() as BattleEventTriggerModel;
    }

    public void Reset()
    {
        BattleEventData = default;
        TriggerType = BattleEventTriggerType.None;
        TargetType = BattleEventTargetType.None;
        Direction = Vector2.zero;
        TargetCount = 0;
        Sender = null;
        PrefabName = string.Empty;
        Range = 0f;
        Speed = 0f;
    }

    public void SetInfoByData(DataAbility abilityData, int level)
    {
        if (abilityData.IsNull)
            return;

        AbilityDataId = abilityData.Id;
        Level = level;
        TriggerType = abilityData.TriggerType;
        TargetType = abilityData.TargetType;
        PrefabName = abilityData.PrefabName;
        HitEffectPrefabName = abilityData.HitEffectPrefabName;
        TargetCount = abilityData.TargetCount[level];
        Range = abilityData.Range[level];
        Speed = abilityData.Speed[level];
        Duration = abilityData.Duration[level];
        Scale = abilityData.Scale[level];

        if (BattleEventData.IsNull)
        {
            BattleEventData = DataManager.Instance.
                                GetDataById<DataBattleEvent>((int)abilityData.BattleEvent);
        }
    }
    
    public BattleEventModel CreateBattleEventModel(CharacterUnitModel receiver)
    {
        var battleEventModel = new BattleEventModel();
        battleEventModel.Initialize(Sender, receiver, BattleEventData, Level);

        return battleEventModel;
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
