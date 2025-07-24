using UnityEngine;
using System.Collections.Generic;

public class BattleEventTriggerModel
{
    public int AbilityDataId { get; private set; }
    public int Level { get; private set; }
    public List<DataBattleEvent> BattleEventDatas { get; private set; } = new List<DataBattleEvent>();

    public BattleEventTriggerType TriggerType { get; private set; }

    public HitCountingType HitCountingType { get; private set; }

    public BattleEventTargetType TargetType { get; private set; }

    public CharacterUnitModel Sender { get; private set; }

    public string PrefabName { get; private set; }

    public string HitEffectPrefabName { get; private set; }

    public Vector2 Direction { get; private set; }

    public int HitCount { get; private set; }

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
        BattleEventDatas.Clear();
        TriggerType = BattleEventTriggerType.None;
        TargetType = BattleEventTargetType.None;
        Direction = Vector2.zero;
        HitCount = 0;
        Sender = null;
        PrefabName = string.Empty;
        Range = 0f;
        Speed = 0f;
    }

    public void SetInfoByData(DataAbility abilityData, int level, ScriptableAbilityBalance balance)
    {
        if (abilityData.IsNull)
            return;

        AbilityDataId = abilityData.Id;
        Level = level;
        TriggerType = abilityData.TriggerType;
        TargetType = abilityData.TargetType;
        PrefabName = abilityData.PrefabName;
        HitEffectPrefabName = abilityData.HitEffectPrefabName;
        HitCount = balance.GetHitCount(level);
        Range = balance.GetRange(level);
        Speed = balance.GetSpeed(level);
        Duration = balance.GetDuration(level);
        Scale = balance.GetScale(level);

        if (BattleEventDatas.Count == 0)
        {
            foreach (var battleEventDefine in abilityData.BattleEvents)
            {
                var battleEventData = DataManager.Instance.GetDataById<DataBattleEvent>((int)battleEventDefine);
                if (!battleEventData.IsNull)
                {
                    BattleEventDatas.Add(battleEventData);
                }
            }
        }
    }
    
    public BattleEventModel CreateBattleEventModel(CharacterUnitModel receiver)
    {
        var battleEventModel = new BattleEventModel();
        battleEventModel.Initialize(Sender, receiver, BattleEventDatas[0], Level);

        return battleEventModel;
    }

    public BattleEventModel[] CreateBattleEventModels(CharacterUnitModel receiver)
    {
        var battleEventModels = new BattleEventModel[BattleEventDatas.Count];
        for (int i = 0; i < BattleEventDatas.Count; i++)
        {
            var battleEventModel = new BattleEventModel();
            battleEventModel.Initialize(Sender, receiver, BattleEventDatas[i], Level);
            battleEventModels[i] = battleEventModel;
        }

        return battleEventModels;
    }

    public void SetSender(CharacterUnitModel owner)
    {
        Sender = owner;
    }

    public void SetBattleTargetCount(int value)
    {
        HitCount = value;
    }

    public void SetDirection(Vector3 value)
    {
        Direction = value;
    }
}
