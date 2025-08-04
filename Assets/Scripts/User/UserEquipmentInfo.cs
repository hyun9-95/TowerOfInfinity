using System.Collections.Generic;
using System.Linq;

public class UserEquipmentInfo
{
    #region Property
    public IReadOnlyDictionary<int, Equipment> UserEquipments => userEquipments;
    public IReadOnlyDictionary<EquipmentType, Equipment> EquippedMainCharacterEquipments => equippedMainCharacterEquipments;
    #endregion

    #region Value
    private Dictionary<int, Equipment> userEquipments;
    private Dictionary<EquipmentType, Equipment> equippedMainCharacterEquipments;
    #endregion

    #region Function
    public void CreateFromUserSaveInfo(UserSaveInfo userSaveInfo)
    {
        userEquipments = new Dictionary<int, Equipment>();
        var equipmentContainer = DataManager.Instance.GetDataContainer<DataEquipment>();
        
        foreach (int equipmentId in userSaveInfo.OwnedEquipmentIds)
        {
            int level = userSaveInfo.EquipmentLevels.ContainsKey(equipmentId) ? 
                        userSaveInfo.EquipmentLevels[equipmentId] : 0;
            
            var dataEquipment = equipmentContainer.GetById(equipmentId);

            if (!dataEquipment.IsNull)
                userEquipments[equipmentId] = new Equipment(dataEquipment, level);
        }

        equippedMainCharacterEquipments = new Dictionary<EquipmentType, Equipment>();
        
        foreach (var kvp in userSaveInfo.EquippedMainCharacterEquipmentIds)
        {
            var equipment = GetEquipment(kvp.Value);
            if (equipment != null)
                equippedMainCharacterEquipments[kvp.Key] = equipment;
        }
    }


    public void AddEquipment(int equipmentId, int level)
    {
        if (!userEquipments.ContainsKey(equipmentId))
        {
            var equipmentContainer = DataManager.Instance.GetDataContainer<DataEquipment>();
            var dataEquipment = equipmentContainer.GetById(equipmentId);
            if (!dataEquipment.IsNull)
                userEquipments[equipmentId] = new Equipment(dataEquipment, level);
        }
    }

    public void RemoveEquipment(int equipmentId)
    {
        userEquipments.Remove(equipmentId);
    }

    public Equipment GetEquipment(int equipmentId)
    {
        return userEquipments.TryGetValue(equipmentId, out Equipment equipment) ? equipment : null;
    }

    public bool HasEquipment(int equipmentId)
    {
        return userEquipments.ContainsKey(equipmentId);
    }

    public void EquipToMainCharacter(EquipmentType equipmentType, int equipmentId)
    {
        var equipment = GetEquipment(equipmentId);
        if (equipment == null)
            return;

        equippedMainCharacterEquipments[equipmentType] = equipment;
    }

    public void UnequipFromMainCharacter(EquipmentType equipmentType)
    {
        equippedMainCharacterEquipments.Remove(equipmentType);
    }

    public Equipment GetMainCharacterEquippedEquipment(EquipmentType equipmentType)
    {
        if (equippedMainCharacterEquipments.ContainsKey(equipmentType))
            return equippedMainCharacterEquipments[equipmentType];
        
        return null;
    }

    public Dictionary<EquipmentType, EquipmentDefine> GetMainCharacterEquipments()
    {
        var result = new Dictionary<EquipmentType, EquipmentDefine>();
        
        foreach (var kvp in equippedMainCharacterEquipments)
        {
            result[kvp.Key] = (EquipmentDefine)kvp.Value.DataId;
        }
        
        return result;
    }
    #endregion
}