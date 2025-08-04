public class Equipment
{
    #region Property
    public int DataId { get; private set; }
    public int Level { get; private set; }
    public EquipmentType EquipmentType { get; private set; }
    public LocalizationDefine Name { get; private set; }
    public CharacterPartsDefine PartsData { get; private set; }
    public AbilityDefine Ability { get; private set; }
    #endregion

    #region Function
    public Equipment(DataEquipment dataEquipment, int level)
    {
        DataId = dataEquipment.Id;
        Level = level;
        EquipmentType = dataEquipment.EquipmentType;
        Name = dataEquipment.Name;
        PartsData = dataEquipment.PartsData;
        Ability = dataEquipment.Ability;
    }

    public void SetLevel(int level)
    {
        if (level < 1)
            return;
            
        Level = level;
    }
    #endregion
}