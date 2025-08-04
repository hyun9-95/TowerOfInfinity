using System.Collections.Generic;
using System.Linq;

public class UserEquipmentInfo
{
    #region Property
    public List<Equipment> OwnedEquipments { get; private set; }
    public Dictionary<EquipmentType, Equipment> EquippedMainCharacterEquipments { get; private set; }
    #endregion

    #region Function
    public void CreateFromUserSaveInfo(UserSaveInfo userSaveInfo)
    {
        OwnedEquipments = new List<Equipment>();
        var equipmentContainer = DataManager.Instance.GetDataContainer<DataEquipment>();
        
        foreach (int equipmentId in userSaveInfo.OwnedEquipmentIds)
        {
            int level = userSaveInfo.EquipmentLevels.ContainsKey(equipmentId) ? 
                        userSaveInfo.EquipmentLevels[equipmentId] : 0;
            
            var dataEquipment = equipmentContainer.GetById(equipmentId);

            if (!dataEquipment.IsNull)
                OwnedEquipments.Add(new Equipment(dataEquipment, level));
        }

        EquippedMainCharacterEquipments = new Dictionary<EquipmentType, Equipment>();
        
        foreach (var kvp in userSaveInfo.EquippedMainCharacterEquipmentIds)
        {
            var equipment = GetEquipment(kvp.Value);
            if (equipment != null)
                EquippedMainCharacterEquipments[kvp.Key] = equipment;
        }
    }


    public void AddEquipment(int equipmentId, int level)
    {
        var existingEquipment = GetEquipment(equipmentId);

        if (existingEquipment == null)
        {
            var equipmentContainer = DataManager.Instance.GetDataContainer<DataEquipment>();
            var dataEquipment = equipmentContainer.GetById(equipmentId);
            if (!dataEquipment.IsNull)
                OwnedEquipments.Add(new Equipment(dataEquipment, level));
        }
    }

    public void RemoveEquipment(int equipmentId)
    {
        var equipment = GetEquipment(equipmentId);

        if (equipment != null)
            OwnedEquipments.Remove(equipment);
    }

    public Equipment GetEquipment(int equipmentId)
    {
        return OwnedEquipments.FirstOrDefault(e => e.DataId == equipmentId);
    }

    public bool HasEquipment(int equipmentId)
    {
        return GetEquipment(equipmentId) != null;
    }

    public void EquipToMainCharacter(EquipmentType equipmentType, int equipmentId)
    {
        var equipment = GetEquipment(equipmentId);
        if (equipment == null)
            return;

        EquippedMainCharacterEquipments[equipmentType] = equipment;
    }

    public void UnequipFromMainCharacter(EquipmentType equipmentType)
    {
        EquippedMainCharacterEquipments.Remove(equipmentType);
    }

    public Equipment GetEquippedEquipment(EquipmentType equipmentType)
    {
        if (EquippedMainCharacterEquipments.ContainsKey(equipmentType))
            return EquippedMainCharacterEquipments[equipmentType];
        
        return null;
    }

    public Dictionary<EquipmentType, EquipmentDefine> GetMainCharacterEquipmentDic()
    {
        var result = new Dictionary<EquipmentType, EquipmentDefine>();
        
        foreach (var kvp in EquippedMainCharacterEquipments)
        {
            result[kvp.Key] = (EquipmentDefine)kvp.Value.DataId;
        }
        
        return result;
    }
    #endregion
}