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
    private bool isUpdate = false;
    #endregion

    #region Function
    public void Initialize(CharacterUnitModel owner)
    {
        this.owner = owner;

        abilitySetByCasting.Clear();
        abilityDicById.Clear();
        abilitySlotDic.Clear();
        isUpdate = false;
    }

    public void Start()
    {
        isUpdate = true;
    }

    public void AddAbility(int newAbilityDataId)
    {
        var newAbility = AbilityFactory.Create<Ability>(newAbilityDataId, owner);

        if (newAbility == null)
        {
            Logger.Error($"Add Ability Failed : {newAbilityDataId}");
            return;
        }    

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
        if (!isUpdate)
            return;

        // 모든 어빌리티 쿨타임 갱신
        foreach (var ability in abilityDicById.Values)
            ability.ReduceCoolTime();

        Cast(CastingType.Auto);
    }

    public void Cast(CastingType castingType)
    {
        if (!abilitySetByCasting.TryGetValue(castingType, out var abilities))
            return;

        foreach (var ability in abilities)
        {
            if (ability.IsCastable)
                ability.Cast();
        }
    }

    public void DelayCast(CastingType castingType, float delay)
    {
        if (!abilitySetByCasting.TryGetValue(castingType, out var abilities))
            return;

        foreach (var ability in abilities)
        {
            if (ability.IsCastable)
                ability.DelayCast(delay).Forget();
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

    public IReadOnlyDictionary<AbilitySlotType, List<Ability>> GetAbilitySlotDic()
    {
        return abilitySlotDic;
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

    public bool IsDrawable(int dataId)
    {
        if (!abilityDicById.TryGetValue(dataId, out var ability))
            return true;

        if (ability.Model.Level >= IntDefine.MAX_ABILITY_LEVEL)
            return false;

        return true;
    }

    public bool IsContain(int dataId)
    {
        return abilityDicById.ContainsKey(dataId);
    }

    public int GetLevel(int dataId)
    {
        if (!abilityDicById.TryGetValue(dataId, out var ability))
            return 0;

        return ability.Model.Level;
    }

    public void Cancel()
    {
        isUpdate = false;

        DelayClear().Forget();
    }

    private async UniTask DelayClear()
    {
        await UniTaskUtils.WaitForLastUpdate();

        abilityDicById.Clear();
        abilitySetByCasting.Clear();
        abilitySlotDic.Clear();
    }
    #endregion

}
