using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameManager : BaseMonoManager<GameManager>
{
    #region Property
    #endregion
    [SerializeField]
    private LoadDataType loadDataType;

    [SerializeField]
    private CheatConfig cheatConfig;

    private void Awake()
    {
        DontDestroyOnLoad(this);

#if CHEAT
        CheatManager.instance.SetCheatConfig(cheatConfig);
#endif
    }

    private void Start()
    {
        IntroFlowModel introFlowModel = new IntroFlowModel();
        introFlowModel.SetLoadDataType(loadDataType);   
        
        FlowManager.Instance.ChangeFlow(FlowType.IntroFlow, introFlowModel).Forget();
    }
}