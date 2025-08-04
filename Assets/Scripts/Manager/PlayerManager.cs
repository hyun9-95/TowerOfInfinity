#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using UnityEngine;

public class PlayerManager : BaseMonoManager<PlayerManager>
{
    #region Property
    public User MyUser { get; private set; }

    public MainPlayerCharacter MainPlayerCharacter => mainPlayerCharacter;
    #endregion

    #region Value
    [SerializeField]
    private Transform playerCharacterTransform;

    private MainPlayerCharacter mainPlayerCharacter;
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

    public async UniTask<CharacterUnit> GetMainCharacter()
    {
        if (mainPlayerCharacter == null)
        {
            mainPlayerCharacter = await AddressableManager.Instance.InstantiateAddressableMonoAsync<MainPlayerCharacter>
                (MyUser.UserCharacterInfo.MainCharacterInfo.MainCharacterPath, playerCharacterTransform);
        }

        await mainPlayerCharacter.UpdateModel(MyUser.UserCharacterInfo.MainCharacterInfo);

        return mainPlayerCharacter.CharacterUnit;
    }
}