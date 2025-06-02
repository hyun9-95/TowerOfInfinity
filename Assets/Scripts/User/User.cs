public class User
{
    #region Property
    public UserSaveInfo UserSaveInfo { get; private set; }
    public UserCharacter[] UserCharacters { get; private set; }
    public UserCharacter LeaderCharacter { get; private set; }
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

            UserCharacter userCharacter = new UserCharacter();
            userCharacter.SetCharacterDataId(dataCharacterId);
            userCharacter.SetWeaponDataId(weaponDataId);
            userCharacter.SetActiveSkillDataId(activeSkillDataId);
            userCharacter.SetPassiveSkillDataId(passiveSkillDataId);

            UserCharacters[i] = userCharacter;

            if (userCharacter.CharacterDataId == userSaveInfo.LeaderCharacterDataId)
                LeaderCharacter = userCharacter;
        }

        if (LeaderCharacter == null)
            LeaderCharacter = UserCharacters[0];
    }
    #endregion
}
