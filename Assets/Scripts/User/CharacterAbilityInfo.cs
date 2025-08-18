using System.Collections.Generic;
using UnityEngine;

public class CharacterAbilityInfo
{
    #region Property
    #endregion

    #region Value
    private Dictionary<AbilitySlotType, AbilityDefine> abilitySlotDic = new();
    #endregion

    #region Function
    public void SetAbilityByCharacterData(DataCharacter charData)
    {
        SetAbility(AbilitySlotType.Weapon, charData.PrimaryWeaponAbility);
        SetAbility(AbilitySlotType.Active, charData.ActiveSkill);
        SetAbility(AbilitySlotType.Passive, charData.PassiveSkill);
    }

    public void SetAbility(AbilitySlotType slotType, AbilityDefine ability)
    {
        abilitySlotDic[slotType] = ability;
    }

    public IEnumerable<AbilityDefine> GetAllAbilityDefines()
    {
        if (abilitySlotDic == null)
            return null;

        return abilitySlotDic.Values;
    }

    public AbilityDefine GetAbility(AbilitySlotType slotType)
    {
        if (abilitySlotDic.TryGetValue(slotType, out var abilityDefine))
            return abilityDefine;

        return abilitySlotDic[slotType];
    }
    #endregion
}
