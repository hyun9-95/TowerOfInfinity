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
    public CharacterStateActionHandler ActionHandler { get; private set; }
    public Transform Transform { get; private set; }
    public CharacterUnitModel Target { get; private set; }
    public InputWrapper InputWrapper { get; private set; } 
    public NavMeshAgent Agent { get; private set; }
    public BattleEventProcessorWrapper EventProcessorWrapper { get; private set; }
    public int PendingWeaponCount => pendingWeapons != null ? pendingWeapons.Count : 0;
    public bool IsDead => Hp <= 0;
    public Weapon DefaultWeapon { get; private set; }
    public ActiveSkill ActiveSkill { get; private set; }
    public PassiveSkill PassiveSkill { get; private set; }
    public float Hp { get; private set; }
    public float Attack { get; private set; }
    public float Defense { get; private set; }
    public float MoveSpeed { get; private set; }
    public PathFindType PathFindType { get; private set; }
    public bool IsFlipX { get; private set; }
    public HashSet<Weapon> Weapons => weapons;
    public float RepathTimer { get; private set; }
    #endregion

    #region Value
    private ScriptableCharacterStat baseStat;
    private HashSet<Weapon> weapons;
    private Queue<Weapon> pendingWeapons;
    private Dictionary<StatType, float> statModifiers = new Dictionary<StatType, float>();
    #endregion
    public void SetCharacterDataId(int id)
    {
        CharacterDataId = id;
        CharacterDefine = (CharacterDefine)id;
    }

    public void SetActionHandler(CharacterStateActionHandler handler)
    {
        ActionHandler = handler;
    }

    public void SetEventProcessorWrapper(BattleEventProcessorWrapper wrapper)
    {
        EventProcessorWrapper = wrapper;
    }

    public void EnableInput(bool value)
    {
        InputWrapper = value ? new InputWrapper() : null;
    }

    public void SetTeamTag(TeamTag teamTag)
    {
        TeamTag = teamTag;
    }

    public void InitializeStat(ScriptableCharacterStat scriptableBaseStat)
    {
        baseStat = scriptableBaseStat;

        Hp = baseStat.GetStat(StatType.MaxHp).Value;
        
        foreach (StatType statType in Enum.GetValues(typeof(StatType)))
        {
            if (statType == StatType.Max)
                continue;

            statModifiers[statType] = 0;
        }
    }

    public void SetIsFlipX(bool value)
    {
        IsFlipX = value;
    }

    public void ReduceHp(float value)
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

    public void SetDefaultWeapon(Weapon weapon)
    {
        DefaultWeapon = weapon;
    }

    public void AddWeapon(Weapon weapon)
    {
        if (weapons == null)
            weapons = new HashSet<Weapon>();

        if (weapons.Contains(weapon))
            return;

        weapons.Add(weapon);

        if (!weapon.IsProcessing)
        {
            if (pendingWeapons == null)
                pendingWeapons = new Queue<Weapon>();

            pendingWeapons.Enqueue(weapon);
        }
    }

    public void RemoveWeapon(Weapon weapon)
    {
        if (weapons == null)
            return;

        weapons.Remove(weapon);
    }

    public void SetActiveSkill(ActiveSkill activeSkill)
    {
        ActiveSkill = activeSkill;
    }

    public void SetPassiveSkill(PassiveSkill passiveSkill)
    {
        PassiveSkill = passiveSkill;
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

    public Weapon DequeuePendingWeapon()
    {
        if (pendingWeapons.Count == 0)
            return null;

        var weapon = pendingWeapons.Dequeue();

        if (weapon != null && weapons.Contains(weapon))
            return weapon;

        return null;
    }

    public IEnumerable<Weapon> GetAllWeapons()
    {
        if (weapons == null)
            return null;

        return weapons;
    }
}