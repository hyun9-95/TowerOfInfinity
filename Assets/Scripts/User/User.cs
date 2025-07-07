using System.Collections.Generic;
using System.ComponentModel.Design;

public class User
{
    #region Property
    public UserSaveInfo UserSaveInfo { get; private set; }
    public UserCharacter[] UserCharacters { get; private set; }
    public UserCharacter[] UserTeams { get; private set; } = new UserCharacter[3];
    public UserCharacter LeaderCharacter
    {
        get
        {
            if (UserTeams == null || UserTeams.Length == 0)
                return null;

            return UserTeams[0];
        }
    }
    #endregion

    #region Value

    #endregion

    #region Function
    public void CreateUserByUserSaveInfo(UserSaveInfo userSaveInfo)
    {
        UserSaveInfo = userSaveInfo;

        CreateUserCharacters(userSaveInfo);
    }

    private void CreateUserCharacters(UserSaveInfo userSaveInfo)
    {
        UserCharacters = new UserCharacter[userSaveInfo.CharacterDataIds.Length];

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

            UserCharacter userCharacter = new UserCharacter();
            userCharacter.SetCharacterDataId(dataCharacterId);
            userCharacter.SetWeaponDataId(weaponDataId);
            userCharacter.SetActiveSkillDataId(activeSkillDataId);
            userCharacter.SetPassiveSkillDataId(passiveSkillDataId);
            userCharacter.SetSlotIndex(userSaveInfo.CharacterSlotIndexDic[i]);

            UserCharacters[i] = userCharacter;

            if (slotIndex > 0 && slotIndex < UserTeams.Length)
            {
                if (UserTeams[i] != null)
                {
                    Logger.Error($"중복 인덱스 ! => {userCharacter.CharacterDataId}");
                    continue;
                }

                UserTeams[i] = userCharacter;
            }
        }
    }
    #endregion
}
