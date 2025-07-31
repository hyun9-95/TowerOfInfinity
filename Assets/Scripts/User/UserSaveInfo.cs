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
    public int[] CharacterDataIds { get; private set; }

    [JsonProperty]
    public Dictionary<int, int> CharacterWeaponDic { get; private set; }

    [JsonProperty]
    public Dictionary<int, int> CharacterActiveSkillDic { get; private set; }

    [JsonProperty]
    public Dictionary<int, int> CharacterPassiveSkillDic { get; private set; }

    [JsonProperty]
    public Dictionary<int, int> CharacterSlotIndexDic { get; private set; }

    [JsonProperty]
    public CharacterRace CharacterRace { get; private set; }

    [JsonProperty]
    public int HairPartsId { get; private set; }

    [JsonProperty]
    public Dictionary<EquipmentType, EquipmentDefine> EquipmentDic { get; private set; }
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

        if (CharacterDataIds == null || CharacterDataIds.Length == 0)
        {
            CharacterDataIds = new int[]
            {
                IntDefine.DEFAULT_CHARACTER_PLAYER_ID,
            };
        }

        if (CharacterWeaponDic == null)
            CharacterWeaponDic = new Dictionary<int, int>();

        if (CharacterActiveSkillDic == null)
            CharacterActiveSkillDic = new Dictionary<int, int>();

        if (CharacterPassiveSkillDic == null)
            CharacterPassiveSkillDic = new Dictionary<int, int>();

        if (CharacterSlotIndexDic == null)
            CharacterSlotIndexDic = new Dictionary<int, int>();

        if (HairPartsId == 0)
            HairPartsId = (int)CharacterPartsDefine.PARTS_HAIR_HAIR_HAIR1;

        if (EquipmentDic == null)
        {
            EquipmentDic = new Dictionary<EquipmentType, EquipmentDefine>()
            {
                { EquipmentType.Armor, EquipmentDefine.EQUIPMENT_ARMOR_THIEF_TUNIC },
                { EquipmentType.Helmet, EquipmentDefine.EQUIPMENT_ARMOR_THIEF_HOOD },
                { EquipmentType.Weapon, EquipmentDefine.EQUIPMENT_WEAPON_SHORT_DAGGER },
            };
        }
    }
    #endregion
}
