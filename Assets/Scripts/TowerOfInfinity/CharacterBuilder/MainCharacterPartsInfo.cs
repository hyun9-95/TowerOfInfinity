using System.Collections.Generic;

public class MainCharacterPartsInfo
{
    public CharacterRace Race { get; private set; }
    public int HairPartsId { get; private set; }
    public DataCharacterParts Hair { get; private set; }
    public Dictionary<CharacterPartsType, DataCharacterParts> PartsInfoDic { get; private set; } = new();

    #region Value
    private DataContainer<DataCharacterParts> partsContainer;
    private DataContainer<DataEquipment> equipmentContainer;
    #endregion

    public void SetByUserSaveInfo(UserSaveInfo userSaveInfo)
    {
        if (partsContainer == null)
            partsContainer = DataManager.Instance.GetDataContainer<DataCharacterParts>();

        PartsInfoDic.Clear();

        // 종족별 고정파츠
        SetRaceParts(userSaveInfo.CharacterRace);

        // 헤어 파츠
        SetHairParts(userSaveInfo.HairPartsId);

        // 장비 파츠
        SetEquipmentParts(userSaveInfo.EquippedMainCharacterEquipmentIds);
    }

    public void SetRaceParts(CharacterRace race)
    {
        Race = race;
        
        var partsContainer = DataManager.Instance.GetDataContainer<DataCharacterParts>();
        
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
            Hair = partsContainer.GetById(HairPartsId);
        }
        else
        {
            Hair = hairData;
        }
        
        PartsInfoDic[CharacterPartsType.Hair] = Hair;
    }

    public void SetEquipmentParts(Dictionary<EquipmentType, int> equipmentInfo)
    {
        if (equipmentContainer == null)
            equipmentContainer = DataManager.Instance.GetDataContainer<DataEquipment>();

        foreach (var equipInfo in equipmentInfo)
        {
            var equipmentData = equipmentContainer.GetById(equipInfo.Value);

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
