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
        CheatManager.instance.SetCheatConfig(cheatConfig);
#endif

        Application.targetFrameRate = 60;

        SelectPlatform();
    }

    private void Start()
    {
        IntroFlowModel introFlowModel = new IntroFlowModel();
        introFlowModel.SetLoadDataType(loadDataType);   
        
        FlowManager.Instance.ChangeFlow(FlowType.IntroFlow, introFlowModel).Forget();
    }

    private void SelectPlatform()
    {
        platform = new Platform();

#if UNITY_STANDALONE
        platform.SetPlatform(PlatformType.StandAlone);
#elif UNITY_ANDROID
        platform.SetPlatformType(PlatformType.Android);
#elif UNITY_IOS
        platform.SetPlatformType(PlatformType.IOS);
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