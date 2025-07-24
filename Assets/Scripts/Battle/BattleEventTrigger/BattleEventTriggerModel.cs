using UnityEngine;
using System.Collections.Generic;

public class BattleEventTriggerModel
{
    public int AbilityDataId { get; private set; }
    public int Level { get; private set; }
    public List<DataBattleEvent> BattleEventDatas { get; private set; } = new List<DataBattleEvent>();
    public BattleEventTriggerType TriggerType { get; private set; }
    public BattleEventTargetType TargetType { get; private set; }
    public TeamTag TargetTeamTag { get; private set; }
    public CharacterUnitModel Sender { get; private set; }
    public string PrefabName { get; private set; }
    public string HitEffectPrefabName { get; private set; }
    public int SendCount { get; private set; }
    public float Range { get; private set; }
    public float Speed { get; private set; }
    public float Scale { get; private set; }
    public float Duration { get; private set; }

    public BattleEventTriggerModel Clone()
    {
        return MemberwiseClone() as BattleEventTriggerModel;
    }

    public void SetSender(CharacterUnitModel owner)
    {
        Sender = owner;
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
        SendCount = balance.GetSendCount(level);
        Range = balance.GetRange(level);
        Speed = balance.GetSpeed(level);
        Duration = balance.GetDuration(level);
        Scale = balance.GetScale(level);

        TargetTeamTag = abilityData.TargetType == BattleEventTargetType.Ally ?
            Sender.TeamTag : GetOppositeTeamTag(Sender.TeamTag);

        if (BattleEventDatas.Count == 0)
        {
            foreach (var battleEventDefine in abilityData.BattleEvents)
            {
                var battleEventData = DataManager.Instance.GetDataById<DataBattleEvent>((int)battleEventDefine);
                
                if (!battleEventData.IsNull)
                    BattleEventDatas.Add(battleEventData);
            }
        }
    }

    public void Reset()
    {
        BattleEventDatas.Clear();
        TriggerType = BattleEventTriggerType.None;
        TargetType = BattleEventTargetType.None;
        SendCount = 0;
        Sender = null;
        PrefabName = string.Empty;
        Range = 0f;
        Speed = 0f;
        Scale = 0f;
        Duration = 0f;
    }

    private TeamTag GetOppositeTeamTag(TeamTag teamTag)
    {
        if (teamTag == TeamTag.Ally)
            return TeamTag.Enemy;

        return TeamTag.Ally;
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
}
