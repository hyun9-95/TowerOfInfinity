using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public class AbilityProcessor
{
    #region Property

    #endregion

    #region Value
    private CharacterUnitModel owner;
    private Dictionary<CastingType, HashSet<Ability>> abilitySetByCasting = new();
    private Dictionary<int, Ability> abilityDicById = new();
    private Dictionary<AbilitySlotType, List<Ability>> abilitySlotDic = new();

    // 한프레임 돌고 Clear되는 것들
    private List<Ability> tempUpdateList = new List<Ability>();
    private List<Ability> tempCastList = new List<Ability>();
    #endregion

    #region Function
    public void SetOwner(CharacterUnitModel owner)
    {
        this.owner = owner;
    }


    public void AddAbility(int newAbilityDataId)
    {
        // 같은 데이터를 쓰는 능력은 중복 불가함
        if (abilityDicById.ContainsKey(newAbilityDataId))
        {
            LevelUpAbility(newAbilityDataId);
            return;
        }

        var newAbility = AbilityFactory.Create<Ability>(newAbilityDataId, owner);

        if (newAbility == null)
            return;

        abilityDicById[newAbilityDataId] = newAbility;

        var castingType = newAbility.CastingType;
        var slotType = newAbility.Model.AbilityData.SlotType;

        if (!abilitySetByCasting.TryGetValue(castingType, out var abilities))
            abilitySetByCasting[castingType] = new HashSet<Ability>();

        if (!abilitySlotDic.TryGetValue(slotType, out var slotAbilities))
            abilitySlotDic[slotType] = new List<Ability>();

        abilitySetByCasting[newAbility.CastingType].Add(newAbility);
        abilitySlotDic[slotType].Add(newAbility);

        // 오토 + 즉발은 최초 추가 시 1회 발동
        if (newAbility.CastingType is CastingType.Instant or CastingType.Auto)
            newAbility.Cast();
    }

    public void RemoveAbility(int removeAbilityDataId)
    {
        if (!abilityDicById.ContainsKey(removeAbilityDataId))
            return;

        var abilityToRemove = abilityDicById[removeAbilityDataId];

        if (abilitySetByCasting.TryGetValue(abilityToRemove.CastingType, out var abilities))
            abilities.Remove(abilityToRemove);

        if (abilitySlotDic.TryGetValue(abilityToRemove.Model.AbilityData.SlotType, out var slotAbilities))
            slotAbilities.Remove(abilityToRemove);

        abilityDicById.Remove(removeAbilityDataId);
    }

    public void LevelUpAbility(int abilityDataId)
    {
        if (!abilityDicById.TryGetValue(abilityDataId, out var ability))
            return;

        ability.Model.LevelUp();
    }

    public void Update()
    {
        tempUpdateList.Clear();
        tempUpdateList.AddRange(abilityDicById.Values);

        // 모든 어빌리티 쿨타임 갱신
        foreach (var ability in tempUpdateList)
            ability.ReduceCoolTime();

        Cast(CastingType.Auto);
    }

    public void Cast(CastingType castingType)
    {
        if (!abilitySetByCasting.TryGetValue(castingType, out var abilities))
            return;

        tempCastList.Clear();
        tempCastList.AddRange(abilities);

        foreach (var ability in tempCastList)
        {
            if (ability.IsCastable)
                ability.Cast();
        }
    }

    public async UniTask CastPrimaryWeapon(float delay)
    {
        var primaryWeapon = GetPrimaryWeapon();

        if (primaryWeapon != null && primaryWeapon.IsCastable)
        {
            await primaryWeapon.DelayCast(delay);
            Cast(CastingType.OnAttack);
        }
    }

    public bool IsPrimaryWeaponSlotReady()
    {
        if (GetPrimaryWeapon() == null)
            return false;   

        return GetPrimaryWeapon().IsCastable;
    }

    public float GetPrimaryWeaponCoolTime()
    {
        if (GetPrimaryWeapon() == null)
            return 0;

        return GetPrimaryWeapon().Model.CoolTime;
    }

    public float GetPrimaryWeaponRange()
    {
        if (GetPrimaryWeapon() == null)
            return 0;

        return GetPrimaryWeapon().Model.Range;
    }

    private Ability GetPrimaryWeapon()
    {
        if (!abilitySlotDic.TryGetValue(AbilitySlotType.Weapon, out var weaponAbilities))
            return null;

        if (weaponAbilities.Count == 0)
            return null;

        return weaponAbilities[0];
    }

    public IReadOnlyList<Ability> GetAbilitiesBySlotType(AbilitySlotType slotType)
    {
        if (!abilitySlotDic.TryGetValue(slotType, out var abilities))
            return null;

        return abilities;
    }

    public void Cancel()
    {
        abilityDicById.Clear();
        abilitySetByCasting.Clear();
        abilitySlotDic.Clear();
    }
    #endregion

}
