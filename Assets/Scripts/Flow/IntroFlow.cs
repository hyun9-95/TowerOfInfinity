#pragma warning disable CS1998
using Cysharp.Threading.Tasks;

public class IntroFlow : BaseFlow<IntroFlowModel>
{
    public override UIType ViewType => UIType.LobbyView;

    public override FlowType FlowType => FlowType.IntroFlow;

    public override async UniTask LoadingProcess()
    {
        await TransitionManager.Instance.In(TransitionType.Default);
        await LoadData(Model.LoadDataType);
        await AbilityFactory.InitializeAbilityBalanceDic();
    }

    private async UniTask LoadData(LoadDataType loadDataType)
    {
        await TransitionManager.Instance.Out(TransitionType.Default);

        IntroController dataLoadingController = new IntroController();
        IntroViewModel viewModel = new IntroViewModel();
        viewModel.SetLoadDataType(loadDataType);

        dataLoadingController.SetModel(viewModel);

        // 로딩은 기본 Resources 경로에 포함
        await UIManager.Instance.ChangeView(dataLoadingController, false);
    }

    public override async UniTask Process()
    {
        LobbyFlowModel lobbyFlowModel = new LobbyFlowModel();
        lobbyFlowModel.SetLobbySceneDefine(SceneDefine.Lobby_Sanctuary);

        await FlowManager.Instance.ChangeFlow(FlowType.LobbyFlow, lobbyFlowModel);
    }

    public override async UniTask Exit()
    {
        await AddressableManager.Instance.UnloadSceneAsync(SceneDefine.IntroScene);
    }

}
