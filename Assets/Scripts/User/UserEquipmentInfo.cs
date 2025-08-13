using System.Collections.Generic;

public class UserEquipmentInfo
{
    #region Property
    public IReadOnlyDictionary<int, Equipment> UserEquipments => userEquipments;
    #endregion

    #region Value
    private Dictionary<int, Equipment> userEquipments;
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

            if (!dataEquipment.IsNullOrEmpty())
                userEquipments[equipmentId] = new Equipment(dataEquipment, level);
        }
    }

    public Dictionary<EquipmentType, Equipment> CreateEquippedMainCharacterEquipments(UserSaveInfo userSaveInfo)
    {
        var equippedEquipments = new Dictionary<EquipmentType, Equipment>();
        
        foreach (var kvp in userSaveInfo.EquippedMainCharacterEquipmentIds)
        {
            var equipment = GetEquipment(kvp.Value);
            if (equipment != null)
                equippedEquipments[kvp.Key] = equipment;
        }
        
        return equippedEquipments;
    }


    public void AddEquipment(int equipmentId, int level)
    {
        if (!userEquipments.ContainsKey(equipmentId))
        {
            var equipmentContainer = DataManager.Instance.GetDataContainer<DataEquipment>();
            var dataEquipment = equipmentContainer.GetById(equipmentId);

            if (!dataEquipment.IsNullOrEmpty())
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
    #endregion
}