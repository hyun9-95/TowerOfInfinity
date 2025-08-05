using UnityEngine;

public class CharacterInfo
{
    #region Property
    public int PrimaryWeaponDataId { get; private set; }
    public int ActiveSkillDataId { get; private set; }
    public int PassiveSkillDataId { get; private set; }
    #endregion

    #region Value
    #endregion

    #region Function
    public void SetPrimaryWeaponAbilityDataId(int abilityWeaponId)
    {
        PrimaryWeaponDataId = abilityWeaponId;
    }

    public void SetActiveSkillDataId(int dataActiveSkillId)
    {
        ActiveSkillDataId = dataActiveSkillId;
    }

    public void SetPassiveSkillDataId(int dataPassiveSkillId)
    {
        PassiveSkillDataId = dataPassiveSkillId;
    }
    #endregion
}
