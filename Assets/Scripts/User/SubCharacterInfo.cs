public class SubCharacterInfo
{
    #region Property
    public int CharacterDataId { get; private set; }
    public int WeaponDataId { get; private set; }
    public int ActiveSkillDataId { get; private set; }  
    public int PassiveSkillDataId { get; private set; }
    public int SlotIndex { get; private set; }
    #endregion

    #region Value
    #endregion

    #region Function
    public void SetCharacterDataId(int dataCharacterId)
    {
        CharacterDataId = dataCharacterId;
    }

    public void SetWeaponDataId(int dataWeaponId)
    {
        WeaponDataId = dataWeaponId;
    }

    public void SetActiveSkillDataId(int dataActiveSkillId)
    {
        ActiveSkillDataId = dataActiveSkillId;
    }

    public void SetPassiveSkillDataId(int dataPassiveSkillId)
    {
        PassiveSkillDataId = dataPassiveSkillId;
    }

    public void SetSlotIndex(int slotIndex)
    {
        SlotIndex = slotIndex;
    }
    #endregion
}
