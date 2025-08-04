using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UserSaveInfo
{
    #region Property
    [JsonProperty]
    public string Id { get; private set; }

    // 추후 도입부로 분리
    [JsonProperty]
    public bool IsCompletePrologue { get; private set; }

    [JsonProperty]
    public List<int> SubCharacterDataIds { get; private set; }

    [JsonProperty]
    public Dictionary<int, int> SubCharacterSlotIndexDic { get; private set; }

    [JsonProperty]
    public CharacterRace CharacterRace { get; private set; }

    [JsonProperty]
    public int HairPartsId { get; private set; }

    [JsonProperty]
    public List<int> OwnedEquipmentIds { get; private set; }

    [JsonProperty]
    public Dictionary<int, int> EquipmentLevels { get; private set; }

    [JsonProperty]
    public Dictionary<EquipmentType, int> EquippedMainCharacterEquipmentIds { get; private set; }
    #endregion

    #region Value
    #endregion

    #region Function
    /// <summary>
    /// 기본값
    /// </summary>
    public void CheckDefaultValue()
    {
        if (string.IsNullOrEmpty(Id))
        {
            Id = SystemInfo.deviceUniqueIdentifier;
        }

        if (SubCharacterDataIds == null)
            SubCharacterDataIds = new List<int>();

        if (SubCharacterSlotIndexDic == null)
            SubCharacterSlotIndexDic = new Dictionary<int, int>();

        if (HairPartsId == 0)
            HairPartsId = (int)CharacterPartsDefine.PARTS_HAIR_HAIR_HAIR1;

        if (OwnedEquipmentIds == null)
            OwnedEquipmentIds = new List<int>();

        if (EquipmentLevels == null)
            EquipmentLevels = new Dictionary<int, int>();

        if (EquippedMainCharacterEquipmentIds == null || EquippedMainCharacterEquipmentIds.Count == 0)
        {
            EquippedMainCharacterEquipmentIds = new Dictionary<EquipmentType, int>()
            {
                { EquipmentType.Armor, (int)EquipmentDefine.EQUIPMENT_ARMOR_THIEF_TUNIC },
                { EquipmentType.Helmet, (int)EquipmentDefine.EQUIPMENT_ARMOR_THIEF_HOOD },
                { EquipmentType.Weapon, (int)EquipmentDefine.EQUIPMENT_WEAPON_SHORT_DAGGER },
            };
        }
    }

    public void SetId(string id) => Id = id;
    public void SetIsCompletePrologue(bool isCompletePrologue) => IsCompletePrologue = isCompletePrologue;
    public void SetSubCharacterDataIds(List<int> subCharacterDataIds) => SubCharacterDataIds = subCharacterDataIds;
    public void SetSubCharacterSlotIndexDic(Dictionary<int, int> subCharacterSlotIndexDic) => SubCharacterSlotIndexDic = subCharacterSlotIndexDic;
    public void SetCharacterRace(CharacterRace characterRace) => CharacterRace = characterRace;
    public void SetHairPartsId(int hairPartsId) => HairPartsId = hairPartsId;
    public void SetOwnedEquipmentIds(List<int> ownedEquipmentIds) => OwnedEquipmentIds = ownedEquipmentIds;
    public void SetEquipmentLevels(Dictionary<int, int> equipmentLevels) => EquipmentLevels = equipmentLevels;
    public void SetEquippedMainCharacterEquipmentIds(Dictionary<EquipmentType, int> equippedMainCharacterEquipmentIds) => EquippedMainCharacterEquipmentIds = equippedMainCharacterEquipmentIds;
    #endregion
}
