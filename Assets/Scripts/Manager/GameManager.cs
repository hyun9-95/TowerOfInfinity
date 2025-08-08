using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameManager : BaseMonoManager<GameManager>
{
    #region Property
    public static GameSettings Settings => Instance.settings;


    #endregion
    [SerializeField]
    private LoadDataType loadDataType;


    private GameSettings settings;


    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    /// <summary>
    /// User 생성 후, 저장된 세팅 정보를 로드
    /// </summary>
    public void LoadGameSettings()
    {
        LoadGameSetting();
        InitializeLocalization();
    }

    private void LoadGameSetting()
    {
        settings = new GameSettings();
        settings.LoadSettings();
    }

    private void InitializeLocalization()
    {
        var applyLocal = settings.Localization.Type;

#if UNITY_EDITOR
        if (CheatManager.CheatConfig.testLocalType != LocalizationType.None)
            applyLocal = CheatManager.CheatConfig.testLocalType;
#endif

        if (applyLocal == LocalizationType.None)
            applyLocal = LocalizationType.English;

        LocalizationManager.Instance.SetLocalizationType(applyLocal);
    }

    private void Start()
    {
        IntroFlowModel introFlowModel = new IntroFlowModel();
        introFlowModel.SetLoadDataType(loadDataType);   
        
        FlowManager.Instance.ChangeFlow(FlowType.IntroFlow, introFlowModel).Forget();
    }
}