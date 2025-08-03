using System;

public class User
{
    #region Property
    public int ID { get; set; }
    public UserSaveInfo UserSaveInfo { get; private set; }
    public SubCharacterInfo[] SubCharacterInfos { get; private set; }
    public SubCharacterInfo[] CurrentDeck { get; private set; } = new SubCharacterInfo[3];
    public MainCharacterPartsInfo UserCharacterPartsInfo { get; private set; }
    #endregion

    #region Value

    #endregion

    #region Function
    public void CreateUserByUserSaveInfo(UserSaveInfo userSaveInfo)
    {
        UserSaveInfo = userSaveInfo;

        CreateUserPartsInfo(userSaveInfo);
        CreateSubCharacters(userSaveInfo);
    }

    private void CreateSubCharacters(UserSaveInfo userSaveInfo)
    {
        SubCharacterInfos = new SubCharacterInfo[userSaveInfo.CharacterDataIds.Length];

        Array.Sort(userSaveInfo.CharacterDataIds, (a, b) => a.CompareTo(b));
        
        for (int i = 0; i < userSaveInfo.CharacterDataIds.Length; i++)
        {
            int dataCharacterId = userSaveInfo.CharacterDataIds[i];

            int weaponDataId = userSaveInfo.CharacterWeaponDic.ContainsKey(dataCharacterId) ?
                userSaveInfo.CharacterWeaponDic[dataCharacterId] : 0;

            int activeSkillDataId = userSaveInfo.CharacterActiveSkillDic.ContainsKey(dataCharacterId) ?
                userSaveInfo.CharacterActiveSkillDic[dataCharacterId] : 0;

            int passiveSkillDataId = userSaveInfo.CharacterPassiveSkillDic.ContainsKey(dataCharacterId) ?
                userSaveInfo.CharacterPassiveSkillDic[dataCharacterId] : 0;

            int slotIndex = userSaveInfo.CharacterSlotIndexDic.ContainsKey(dataCharacterId) ?
                userSaveInfo.CharacterSlotIndexDic[dataCharacterId] : -1;

            SubCharacterInfo userCharacter = new SubCharacterInfo();
            userCharacter.SetCharacterDataId(dataCharacterId);
            userCharacter.SetWeaponDataId(weaponDataId);
            userCharacter.SetActiveSkillDataId(activeSkillDataId);
            userCharacter.SetPassiveSkillDataId(passiveSkillDataId);
            userCharacter.SetSlotIndex(slotIndex);

            SubCharacterInfos[i] = userCharacter;

            if (slotIndex >= 0 && slotIndex < CurrentDeck.Length)
            {
                if (CurrentDeck[i] != null)
                {
                    Logger.Error($"중복 인덱스 ! => {userCharacter.CharacterDataId}");
                    continue;
                }

                CurrentDeck[i] = userCharacter;
            }
        }
    }

    private void CreateUserPartsInfo(UserSaveInfo userSaveInfo)
    {
        UserCharacterPartsInfo = new MainCharacterPartsInfo();
        UserCharacterPartsInfo.SetByUserSaveInfo(userSaveInfo);
    }
    #endregion
}
