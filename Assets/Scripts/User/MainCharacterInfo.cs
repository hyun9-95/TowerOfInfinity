using System.Collections.Generic;
using UnityEngine;

public class MainCharacterInfo : CharacterInfo
{
    #region Property
    public string MainCharacterPath => PathDefine.CHARACTER_MAIN_CHARACTER_PATH;
    public MainCharacterPartsInfo PartsInfo { get; private set; } = new MainCharacterPartsInfo();
    public CharacterRace CharacterRace { get; private set; }
    public int HairPartsId { get; private set; }
    public IReadOnlyDictionary<EquipmentType, Equipment> EquippedEquipments => equippedEquipments;
    #endregion

    #region Value
    private Dictionary<EquipmentType, Equipment> equippedEquipments = new Dictionary<EquipmentType, Equipment>();
    #endregion

    #region Function
    public void SetCharacterRace(CharacterRace characterRace)
    {
        CharacterRace = characterRace;
    }

    public void SetHairPartsId(int hairDataId)
    {
        HairPartsId = hairDataId;
    }

    public void SetPartsInfo(MainCharacterPartsInfo partsInfo)
    {
        PartsInfo = partsInfo;
    }

    public void EquipEquipment(Equipment equipment)
    {
        equippedEquipments[equipment.EquipmentType] = equipment;
    }

    public void UnequipEquipment(EquipmentType equipmentType)
    {
        equippedEquipments.Remove(equipmentType);
    }

    public Equipment GetEquippedEquipment(EquipmentType equipmentType)
    {
        return equippedEquipments.TryGetValue(equipmentType, out Equipment equipment) ? equipment : null;
    }

    public Dictionary<EquipmentType, EquipmentDefine> GetEquipmentDefines()
    {
        var result = new Dictionary<EquipmentType, EquipmentDefine>();
        
        foreach (var kvp in equippedEquipments)
        {
            result[kvp.Key] = (EquipmentDefine)kvp.Value.DataId;
        }
        
        return result;
    }

    public void SetEquippedEquipments(Dictionary<EquipmentType, Equipment> equipments)
    {
        equippedEquipments = equipments ?? new Dictionary<EquipmentType, Equipment>();
    }
    #endregion
}
