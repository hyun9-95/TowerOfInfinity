#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using UnityEngine;

public class PlayerManager : BaseMonoManager<PlayerManager>
{
    #region Property
    public User MyUser { get; private set; }
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

    public async UniTask LoadMainPlayerCharacter()
    {
        if (mainPlayerCharacter == null)
        {
            var mainPlayerCharacterPath = MyUser.UserCharacterInfo.MainCharacterInfo.MainCharacterPath;
            
            mainPlayerCharacter = await AddressableManager.Instance.InstantiateUntrackedAsync<MainPlayerCharacter>
                (mainPlayerCharacterPath, playerCharacterTransform);

            await CharacterFactory.Instance.SetCharacterScriptableInfo
                (mainPlayerCharacter.CharacterUnit, CharacterType.Main);
        }
    }

    /// <summary>
    /// Update를 임시로 멈추고 모델을 업데이트한다.
    /// 사용하려면 다시 활성화해줘야함
    /// </summary>
    /// <returns></returns>
    public async UniTask UpdateMainPlayerCharacter(CharacterSetUpType setUpType, bool isStopUnitUpdate = true)
    {
        if (isStopUnitUpdate)
            mainPlayerCharacter.CharacterUnit.StopUpdate();

        await mainPlayerCharacter.UpdateMainCharacter(MyUser.UserCharacterInfo.MainCharacterInfo, setUpType);
    }

    public MainPlayerCharacter GetMainPlayerCharacter()
    {
        return mainPlayerCharacter;
    }

    public CharacterUnit GetMainPlayerCharacterUnit()
    {
        if (mainPlayerCharacter == null)
            return null;

        return mainPlayerCharacter.CharacterUnit;
    }

    public MainCharacterInfo GetMainCharacterInfo()
    {
        return MyUser.UserCharacterInfo.MainCharacterInfo;
    }
}