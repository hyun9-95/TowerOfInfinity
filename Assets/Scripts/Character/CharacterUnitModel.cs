using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterUnitModel : IBaseUnitModel
{
    #region Property
    public int CharacterDataId { get; private set; }
    public CharacterDefine CharacterDefine { get; private set; }
    public CharacterAnimState CurrentAnimState { get; private set; }
    public TeamTag TeamTag { get; private set; }
    public CharacterStateActionHandler ActionHandler { get; private set; }
    public Transform Transform { get; private set; }
    public CharacterUnitModel Target { get; private set; }
    public InputWrapper InputWrapper { get; private set; } 
    public NavMeshAgent Agent { get; private set; }
    public int BattleEventCount => battleEventQueue.Count;
    public int PendingWeaponCount => pendingWeapons != null ? pendingWeapons.Count : 0;
    public bool IsDead => Hp <= 0;
    public Weapon DefaultWeapon { get; private set; }
    public ActiveSkill ActiveSkill { get; private set; }
    public PassiveSkill PassiveSkill { get; private set; }
    public float Hp { get; private set; }
    public PathFindType PathFindType { get; private set; }
    public bool IsFlipX { get; private set; }
    public HashSet<Weapon> Weapons => weapons;
    #endregion

    #region Value
    private ScriptableCharacterStat baseStat;

    private Queue<BattleEvent> battleEventQueue = new Queue<BattleEvent>();
    private HashSet<Weapon> weapons;
    private Queue<Weapon> pendingWeapons;
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

    public void EnableInput(bool value)
    {
        InputWrapper = value ? new InputWrapper() : null;
    }

    public void SetTeamTag(TeamTag teamTag)
    {
        TeamTag = teamTag;
    }

    public void SetBaseStat(ScriptableCharacterStat scriptableBaseStat)
    {
        baseStat = scriptableBaseStat;

        Hp = baseStat.GetStat(StatType.MaxHp).Value;
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

    /// <summary>
    /// 버프등 최종 스탯 계산식 추가하기
    /// </summary>
    /// <param name="statType"></param>
    /// <returns></returns>
    public float GetStatValue(StatType statType)
    {
        return baseStat.GetStat(statType).Value;
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

    public void EnqueueBattleEvent(BattleEvent battleEvent)
    {
        if (battleEventQueue == null)
            battleEventQueue = new Queue<BattleEvent>();

        battleEventQueue.Enqueue(battleEvent);
    }

    public BattleEvent DequeueBattleEvent()
    {
        if (battleEventQueue == null)
            return null;

        if (battleEventQueue.Count == 0)
            return null;

        return battleEventQueue.Dequeue();
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
