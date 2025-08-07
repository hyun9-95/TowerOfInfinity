using UnityEngine;

public class CharacterInfo
{
    #region Property
    public AbilityDefine PrimaryWeapon { get; private set; }
    public AbilityDefine ActiveAbility { get; private set; }
    public AbilityDefine PassiveAbility { get; private set; }
    #endregion

    #region Value
    #endregion

    #region Function
    public void SetPrimaryWeaponAbility(AbilityDefine abilityWeapon)
    {
        PrimaryWeapon = abilityWeapon;
    }

    public void SetActiveAbility(AbilityDefine activeAbility)
    {
        ActiveAbility = activeAbility;
    }

    public void SetPassiveAbility(AbilityDefine passiveAbility)
    {
        PassiveAbility = passiveAbility;
    }
    #endregion
}
