using System.Collections.Generic;

public class MainCharacterPartsInfo
{
    public CharacterRace Race { get; private set; }
    public int HairPartsId { get; private set; }
    public bool ShowHelmet { get; private set; }
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
        ShowHelmet = true;
    }

    public void SetRaceParts(CharacterRace race)
    {
        Race = race;
        
        var ids = CommonUtils.GetRacePartsIds(race);

        foreach (var id in ids)
        {
            var data = partsContainer.GetById(id);

            if (data.IsNullOrEmpty())
                continue;

            PartsInfoDic[data.PartsType] = data;
        }
    }

    public void SetHairParts(int hairPartsId)
    {
        HairPartsId = hairPartsId;
        
        var hairData = partsContainer.GetById(hairPartsId);
        PartsInfoDic[CharacterPartsType.Hair] = hairData;
    }

    public void SetShowHelmet(bool value)
    {
        ShowHelmet = value;
    }

    public void SetEquipmentParts(Dictionary<EquipmentType, EquipmentDefine> equipmentInfo)
    {
        foreach (var equipInfo in equipmentInfo)
        {
            if (equipInfo.Key == EquipmentType.Helmet && !ShowHelmet)
                continue;

            var equipmentData = equipmentContainer.GetById((int)equipInfo.Value);

            if (equipmentData.IsNullOrEmpty())
                continue;

            var equipmentPartsData = partsContainer.GetById((int)equipmentData.PartsData);

            if (equipmentPartsData.IsNullOrEmpty())
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
