using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UserSaveInfo
{
    #region Property
    [JsonProperty]
    public string Id { get; private set; }

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
    }
    #endregion
}
