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
    private Ability primaryWeaponAbility;

    // 한프레임 돌고 Clear되는 것들
    private List<Ability> tempUpdateList = new List<Ability>();
    private List<Ability> tempCastList = new List<Ability>();
    #endregion

    #region Function
    public void SetOwner(CharacterUnitModel owner)
    {
        this.owner = owner;
    }

    public void SetPrimaryWeaponAbility(Ability primaryWeaponAbility)
    {
        this.primaryWeaponAbility = primaryWeaponAbility;
    }

    public void AddAbility(int newAbilityDataId)
    {
        // 같은 데이터를 쓰는 능력은 중복 불가함
        if (abilityDicById.ContainsKey(newAbilityDataId))
            return;

        var newAbility = AbilityFactory.Create<Ability>(newAbilityDataId, owner);

        if (newAbility == null)
            return;

        abilityDicById[newAbilityDataId] = newAbility;

        var castingType = newAbility.CastingType;

        if (!abilitySetByCasting.TryGetValue(castingType, out var abilities))
            abilitySetByCasting[castingType] = new HashSet<Ability>();

        abilitySetByCasting[newAbility.CastingType].Add(newAbility);
    }

    public void RemoveAbility(int removeAbilityDataId)
    {
        if (!abilityDicById.ContainsKey(removeAbilityDataId))
            return;

        var abilityToRemove = abilityDicById[removeAbilityDataId];

        if (abilitySetByCasting.TryGetValue(abilityToRemove.CastingType, out var abilities))
            abilities.Remove(abilityToRemove);

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

        // 주무기 쿨타임 갱신
        if (primaryWeaponAbility != null)
            primaryWeaponAbility.ReduceCoolTime();

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
        if (primaryWeaponAbility == null)
            return;

        await primaryWeaponAbility.DelayCast(delay);
        Cast(CastingType.OnAttack);
    }

    public bool IsPrimaryWeaponReady()
    {
        if (primaryWeaponAbility == null)
            return false;

        return primaryWeaponAbility.IsCastable;
    }

    public void Cancel()
    {
        abilityDicById.Clear();
        abilitySetByCasting.Clear();
    }
    #endregion

}
