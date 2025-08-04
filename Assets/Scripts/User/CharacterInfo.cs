using UnityEngine;

public class CharacterInfo
{
    #region Property
    public int DefaultWeaponDataId { get; private set; }
    public int ActiveSkillDataId { get; private set; }
    public int PassiveSkillDataId { get; private set; }
    #endregion

    #region Value
    #endregion

    #region Function
    public void SetDefaultWeaponDataId(int dataWeaponId)
    {
        DefaultWeaponDataId = dataWeaponId;
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
