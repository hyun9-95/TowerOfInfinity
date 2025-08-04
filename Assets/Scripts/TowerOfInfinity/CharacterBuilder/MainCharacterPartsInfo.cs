using System.Collections.Generic;

public class MainCharacterPartsInfo
{
    public CharacterRace Race { get; private set; }
    public int HairPartsId { get; private set; }
    public Dictionary<CharacterPartsType, DataCharacterParts> PartsInfoDic { get; private set; } = new();

    #region Value
    private DataContainer<DataCharacterParts> partsContainer;
    private DataContainer<DataEquipment> equipmentContainer;
    #endregion

    public MainCharacterPartsInfo()
    {
        if (partsContainer == null)
            partsContainer = DataManager.Instance.GetDataContainer<DataCharacterParts>();

        if (equipmentContainer == null)
            equipmentContainer = equipmentContainer = DataManager.Instance.GetDataContainer<DataEquipment>();

        HairPartsId = 0;
        Race = CharacterRace.Human;
    }

    public void SetRaceParts(CharacterRace race)
    {
        Race = race;
        
        var ids = CommonUtils.GetRacePartsIds(race);

        foreach (var id in ids)
        {
            var data = partsContainer.GetById(id);

            if (data.IsNull)
                continue;

            PartsInfoDic[data.PartsType] = data;
        }
    }

    public void SetHairParts(int hairPartsId)
    {
        HairPartsId = hairPartsId;
        
        var hairData = partsContainer.GetById(hairPartsId);

        if (hairData.IsNull)
        {
            HairPartsId = (int)CharacterPartsDefine.PARTS_HAIR_HAIR_HAIR1;
            hairData = partsContainer.GetById(HairPartsId);
        }
        
        PartsInfoDic[CharacterPartsType.Hair] = hairData;
    }

    public void SetEquipmentParts(Dictionary<EquipmentType, EquipmentDefine> equipmentInfo)
    {
        foreach (var equipInfo in equipmentInfo)
        {
            var equipmentData = equipmentContainer.GetById((int)equipInfo.Value);

            if (equipmentData.IsNull)
                continue;

            var equipmentPartsData = partsContainer.GetById((int)equipmentData.PartsData);

            if (equipmentPartsData.IsNull)
                continue;

            PartsInfoDic[equipmentPartsData.PartsType] = equipmentPartsData;
        }
    }

    public DataCharacterParts GetPartsData(CharacterPartsType partsType)
    {
        if (PartsInfoDic.TryGetValue(partsType, out var partsData))
            return partsData;

        return default;
    }
}
