#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerManager : BaseMonoManager<PlayerManager>
{
    #region Property
    public User MyUser { get; private set; }
    #endregion

    public void LoadUser()
    {
        string userSaveInfoPath = Path.Combine(Application.persistentDataPath,
                                        string.Format(PathDefine.PATH_USER_SAVE_INFO, NameDefine.UserSaveInfo));

        string directory = Path.GetDirectoryName(userSaveInfoPath);

        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        UserSaveInfo userSaveInfo = null;

        if (File.Exists(userSaveInfoPath))
        {
            var userSaveInfoJson = File.ReadAllText(userSaveInfoPath);

            if (!string.IsNullOrEmpty(userSaveInfoJson))
                userSaveInfo = JsonConvert.DeserializeObject<UserSaveInfo>(userSaveInfoJson);
        }
 
        // 이래도 없으면 새로 생성
        if (userSaveInfo == null)
            userSaveInfo = new UserSaveInfo();

        userSaveInfo.CheckDefaultValue();

        User user = new User();
        user.CreateUserByUserSaveInfo(userSaveInfo);

        MyUser = user;

        var newSaveInfoJson = JsonConvert.SerializeObject(userSaveInfo);
        File.WriteAllText(userSaveInfoPath, newSaveInfoJson);
    }

    public async UniTask<CharacterUnit> CreateLeaderCharacter(Transform transform)
    {
        if (MyUser == null)
        {
            Logger.Null($"{MyUser}");
            return null;
        }

        return await CreateCharacter(transform, MyUser.LeaderCharacter);
    }

    public async UniTask<List<CharacterUnit>> CreateSubCharacters(Transform transform)
    {
        if (MyUser == null)
        {
            Logger.Null($"{MyUser}");
            return null;
        }

        var characterUnits = new List<CharacterUnit>();

        foreach (var userCharacter in MyUser.UserCharacters)
        {
            if (userCharacter == null)
            {
                Logger.Null($"{userCharacter}");
                continue;
            }

            if (userCharacter == MyUser.LeaderCharacter)
                continue;

            var character = await CreateCharacter(transform, userCharacter);

            if (character != null)
                characterUnits.Add(character);
        }

        return characterUnits;
    }

    private async UniTask<CharacterUnit> CreateCharacter(Transform transform, UserCharacter userCharacter)
    {
        if (MyUser == null)
        {
            Logger.Null($"{MyUser}");
            return null;
        }

        return await CharacterFactory.Instance.CreateCharacter(transform, userCharacter);
    }
}