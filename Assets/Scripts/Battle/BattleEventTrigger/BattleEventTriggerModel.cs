using UnityEngine;
using System.Collections.Generic;

public class BattleEventTriggerModel
{
    public int AbilityDataId { get; private set; }
    public int Level { get; private set; }
    public CharacterUnitModel Sender { get; private set; }
    public List<DataBattleEvent> BattleEventDatas { get; private set; } = new List<DataBattleEvent>();
    public BattleEventTriggerType TriggerType { get; private set; }
    public BattleEventTargetType TargetType { get; private set; }
    public TeamTag TargetTeamTag { get; private set; }
    public string TriggerUnitPath { get; private set; }
    public string HitEffectPrefabName { get; private set; }
    public int HitCount { get; private set; }
    public float Range { get; private set; }
    public float Speed { get; private set; }
    public float Scale { get; private set; }
    public float Duration { get; private set; }
    public int SpawnCount { get; private set; }
    public float HitForce { get; private set; }
    public float SpawnInterval { get; private set; }

    public BattleEventTriggerModel(CharacterUnitModel sender, int level, DataAbility abilityData, ScriptableAbilityBalance balance)
    {
        if (abilityData.IsNullOrEmpty())
            return;

        AbilityDataId = abilityData.Id;
        Sender = sender;

        Level = level;
        TriggerType = abilityData.TriggerType;
        TargetType = abilityData.TargetType;
        TriggerUnitPath = abilityData.TriggerUnitPath;
        HitEffectPrefabName = abilityData.HitEffectPrefabName;
        HitCount = balance.GetHitCount(Level);
        Range = balance.GetRange(Level);
        Speed = balance.GetSpeed(Level);
        Duration = balance.GetDuration(Level);
        Scale = balance.GetScale(Level);
        SpawnCount = balance.GetSpawnCount(Level);
        HitForce = balance.GetHitForce(Level);
        SpawnInterval = balance.GetSpawnInterval(Level);

        TargetTeamTag = abilityData.TargetType == BattleEventTargetType.Ally ?
            Sender.TeamTag : GetOppositeTeamTag(Sender.TeamTag);

        if (BattleEventDatas.Count == 0)
        {
            foreach (var battleEventDefine in abilityData.BattleEvents)
            {
                var battleEventData = DataManager.Instance.GetDataById<DataBattleEvent>((int)battleEventDefine);

                if (!battleEventData.IsNullOrEmpty())
                    BattleEventDatas.Add(battleEventData);
            }
        }
    }

    public void Reset()
    {
        AbilityDataId = 0;
        Level = 0;
        BattleEventDatas.Clear();
        TriggerType = BattleEventTriggerType.None;
        TargetType = BattleEventTargetType.None;
        HitCount = 0;
        Sender = null;
        TriggerUnitPath = string.Empty;
        HitEffectPrefabName = string.Empty;
        TargetTeamTag = TeamTag.Ally;
        Range = 0f;
        Speed = 0f;
        Scale = 0f;
        Duration = 0f;
        SpawnCount = 1;
        HitForce = 0f;
        SpawnInterval = 0f;
    }

    private TeamTag GetOppositeTeamTag(TeamTag teamTag)
    {
        if (teamTag == TeamTag.Ally)
            return TeamTag.Enemy;

        return TeamTag.Ally;
    }
    
    public BattleEventModel CreateBattleEventModel(CharacterUnitModel receiver)
    {
        BattleEventModel battleEventModel = new(Sender, receiver, BattleEventDatas[0], Level);

        return battleEventModel;
    }

    public BattleEventModel[] CreateBattleEventModels(CharacterUnitModel receiver)
    {
        var battleEventModels = new BattleEventModel[BattleEventDatas.Count];
        for (int i = 0; i < BattleEventDatas.Count; i++)
        {
            BattleEventModel battleEventModel = new (Sender, receiver, BattleEventDatas[i], Level);
            battleEventModels[i] = battleEventModel;
        }

        return battleEventModels;
    }
}
