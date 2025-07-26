using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameManager : BaseMonoManager<GameManager>
{
    #region Property
    public static GameSettings Settings => Instance.settings;

    public static Config Config => Instance.config;

    #endregion
    [SerializeField]
    private LoadDataType loadDataType;

    [SerializeField]
    private Config config;

    private GameSettings settings;


    private void Awake()
    {
        DontDestroyOnLoad(this);

        LoadGameSetting();
    }

    private void LoadGameSetting()
    {
        settings = new GameSettings();
        settings.LoadSettings();
    }

    private void Start()
    {
        IntroFlowModel introFlowModel = new IntroFlowModel();
        introFlowModel.SetLoadDataType(loadDataType);   
        
        FlowManager.Instance.ChangeFlow(FlowType.IntroFlow, introFlowModel).Forget();
    }
}