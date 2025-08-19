using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameManager : BaseMonoManager<GameManager>
{
    #region Property
    public static Platform Platform => Instance.platform;
    #endregion
    [SerializeField]
    private LoadDataType loadDataType;

    [SerializeField]
    private PlatformType testPlatformType;

    [SerializeField]
    private CheatConfig cheatConfig;

    private Platform platform;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this);

#if CHEAT
        CheatManager.Instance.SetCheatConfig(cheatConfig);
#endif

        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 1;

        LoadUserSettings();
        SelectPlatform();
    }

    private void Start()
    {
        IntroFlowModel introFlowModel = new IntroFlowModel();
        introFlowModel.SetLoadDataType(loadDataType);
        introFlowModel.SetFlowBGMPath(PathDefine.BGM_TITLE);
        
        FlowManager.Instance.ChangeFlow(FlowType.IntroFlow, introFlowModel).Forget();
    }

    private void LoadUserSettings()
    {
        UserSettings.Load(SystemInfo.deviceUniqueIdentifier);

        var applyLocal = UserSettings.GetLocalizationType();

#if CHEAT
        if (CheatManager.CheatConfig.testLocalType != LocalizationType.None)
            applyLocal = CheatManager.CheatConfig.testLocalType;
#endif

        if (applyLocal == LocalizationType.None)
            applyLocal = LocalizationType.English;

        LocalizationManager.Instance.SetLocalizationType(applyLocal);
    }

    private void SelectPlatform()
    {
        platform = new Platform();

#if UNITY_STANDALONE
        platform.SetPlatform(PlatformType.StandAlone);
#elif UNITY_ANDROID
        platform.SetPlatform(PlatformType.Android);
#elif UNITY_IOS
        platform.SetPlatform(PlatformType.IOS);
#endif

#if UNITY_EDITOR
        if (testPlatformType != PlatformType.None)
            platform.SetPlatform(testPlatformType);
#endif

#if !UNITY_EDITOR
        loadDataType = LoadDataType.Addressable;
#endif
    }
}