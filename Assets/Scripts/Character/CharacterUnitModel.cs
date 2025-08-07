using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterUnitModel : IBaseUnitModel
{
    #region Property
    public int CharacterDataId { get; private set; }
    public int Level { get; private set; }
    public CharacterDefine CharacterDefine { get; private set; }
    public CharacterAnimState CurrentAnimState { get; private set; }
    public TeamTag TeamTag { get; private set; }
    public CharacterType CharacterType { get; private set; }
    public CharacterSetUpType CharacterSetUpType { get; private set; }
    public CharacterActionHandler ActionHandler { get; private set; }
    public Transform Transform { get; private set; }
    public CharacterUnitModel Target { get; private set; }
    public InputWrapper InputWrapper { get; private set; } 
    public NavMeshAgent Agent { get; private set; }
    public BattleEventProcessor EventProcessor { get; private set; }
    public AbilityProcessor AbilityProcessor { get; private set; }
    public CharacterInfo CharacterInfo { get; private set; }
    public bool IsDead => Hp <= 0;
    public float Hp { get; private set; }
    public float Attack { get; private set; }
    public float Defense { get; private set; }
    public float MoveSpeed { get; private set; }
    public PathFindType PathFindType { get; private set; }
    public bool IsFlipX { get; private set; }
    public float RepathTimer { get; private set; }
    public bool IsEnablePhysics { get; private set; }
    #endregion

    #region Value
    private ScriptableCharacterStat baseStat;
    private Dictionary<StatType, float> statModifiers = new Dictionary<StatType, float>();
    private Dictionary<EquipmentType, Equipment> equippedEquipments;
    #endregion
    public void SetCharacterDataId(int id)
    {
        CharacterDataId = id;
        CharacterDefine = (CharacterDefine)id;
    }

    public void SetActionHandler(CharacterActionHandler handler)
    {
        ActionHandler = handler;
    }

    public void SetEventProcessor(BattleEventProcessor processor)
    {
        EventProcessor = processor;
    }

    public void SetAbilityProcessor(AbilityProcessor processor)
    {
        AbilityProcessor = processor;
    }

    public void EnableInput(bool value)
    {
        InputWrapper = value ? new InputWrapper() : null;
    }

    public void SetTeamTag(TeamTag teamTag)
    {
        TeamTag = teamTag;
    }

    public void SetCharacterType(CharacterType characterType)
    {
        CharacterType = characterType;
    }

    public void SetCharacterSetUpType(CharacterSetUpType characterSetUpType)
    {
        CharacterSetUpType = characterSetUpType;
    }

    public void EquipEquipment(Equipment equipment)
    {
        if (equippedEquipments == null)
            equippedEquipments = new Dictionary<EquipmentType, Equipment>();

        equippedEquipments[equipment.EquipmentType] = equipment;
    }

    public void InitializeStat(ScriptableCharacterStat scriptableBaseStat)
    {
        baseStat = scriptableBaseStat;

        Hp = baseStat.GetStat(StatType.MaxHp).Value;
        
        foreach (StatType statType in Enum.GetValues(typeof(StatType)))
        {
            if (statType == StatType.None)
                continue;

            statModifiers[statType] = 0;
        }
    }

    public void SetIsFlipX(bool value)
    {
        IsFlipX = value;
    }

    /// <summary>
    /// BattleSystemManager 외에 호출 금지
    /// </summary>
    /// <param name="value"></param>
    public void AddDamage(float value)
    {
        AddHp(-value);
    }

    public void AddHp(float value)
    {
        Hp += value;

        if (Hp < 0)
        {
            Hp = 0;
        }
        else if (Hp > GetStatValue(StatType.MaxHp))
        {
            Hp = GetStatValue(StatType.MaxHp);
        }
    }

    public Stat GetBaseStat(StatType statType)
    {
        return baseStat.GetStat(statType);
    }

    public float GetStatValue(StatType statType, StatReferenceCondition condition = StatReferenceCondition.BaseStat)
    {
        if (condition == StatReferenceCondition.BaseStat)
            return GetBaseStatValue(statType);

        if (condition == StatReferenceCondition.CurrentStat)
            return GetCurrentStatValue(statType);

        return 0;
    }

    private float GetBaseStatValue(StatType statType)
    {
        return baseStat.GetStat(statType).Value;
    }

    private float GetCurrentStatValue(StatType statType)
    {
        return GetBaseStatValue(statType) + statModifiers[statType];
    }

    public void ChangeStatValue(StatType statType, float value)
    {
        statModifiers[statType] = value;
    }

    public void SetCurrentAnimState(CharacterAnimState animState)
    {
        CurrentAnimState = animState;
    }

    public void SetPathFindType(PathFindType pathFindType)
    {
        PathFindType = pathFindType;
    }

    public void SetCharacterInfo(CharacterInfo characterInfo)
    {
        CharacterInfo = characterInfo;
    }

    public int GetAbilityDataIdBySlot(AbilitySlotType slotType)
    {
        if (CharacterInfo == null)
            return 0;

        return slotType switch
        {
            AbilitySlotType.Weapon => (int)CharacterInfo.PrimaryWeapon,
            AbilitySlotType.Active => (int)CharacterInfo.ActiveAbility,
            AbilitySlotType.Passive => (int)CharacterInfo.PassiveAbility,
            _ => 0
        };
    }

    public bool IsAttackState()
    {
        return CurrentAnimState switch
        {
            CharacterAnimState.Attack => true,
            CharacterAnimState.Jab => true,
            CharacterAnimState.BowShot => true,
            CharacterAnimState.Slash => true,
            _ => false
        };
    }

    public Equipment GetEquipment(EquipmentType equipmentType)
    {
        if (equippedEquipments == null)
            return null;

        if (!equippedEquipments.TryGetValue(equipmentType, out var equipment))
            return null;

        return equipment;
    }

    public void SetTransform(Transform transform)
    {
        Transform = transform;
    }

    public void SetTarget(CharacterUnitModel target)
    {
        Target = target;
    }

    public void SetAgent(NavMeshAgent agent)
    {
        Agent = agent;
    }

    public void SetRepathTimer(float timer)
    {
        RepathTimer = timer;
    }

    public void SetIsEnablePhysics(bool value)
    {
        IsEnablePhysics = value;
    }
}