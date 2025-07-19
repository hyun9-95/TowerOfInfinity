#pragma warning disable CS1998
using Cysharp.Threading.Tasks;

public class IntroFlow : BaseFlow<IntroFlowModel>
{
    public override UIType ViewType => UIType.LobbyView;

    public override FlowType FlowType => FlowType.IntroFlow;

    public override async UniTask LoadingProcess()
    {
    }

    private async UniTask ShowIntroView(LoadDataType loadDataType)
    {
        IntroController IntroController = new IntroController();
        IntroViewModel viewModel = new IntroViewModel();
        viewModel.SetLoadDataType(loadDataType);
        viewModel.SetOnComplteLoading(OnCompleteLoading);

        IntroController.SetModel(viewModel);

        // 로딩은 기본 Resources 경로에 포함
        await UIManager.Instance.ChangeView(IntroController, false);
    }

    public override async UniTask Process()
    {
        ShowIntroView(Model.LoadDataType).Forget();
    }

    private void OnCompleteLoading()
    {
        LobbyFlowModel lobbyFlowModel = new LobbyFlowModel();
        lobbyFlowModel.SetLobbySceneDefine(SceneDefine.Lobby_Sanctuary);

        FlowManager.Instance.ChangeFlow(FlowType.LobbyFlow, lobbyFlowModel).Forget();
    }

    public override async UniTask Exit()
    {
        await AddressableManager.Instance.UnloadSceneAsync(SceneDefine.IntroScene);
    }
}
