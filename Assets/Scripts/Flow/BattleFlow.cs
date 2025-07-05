#pragma warning disable CS1998
using Cysharp.Threading.Tasks;

public class BattleFlow : BaseFlow<BattleFlowModel>
{
    public override UIType ViewType => UIType.LobbyView;
    public override FlowType FlowType => FlowType.BattleFlow;

    private BattleSceneManager battleSceneManager;
    private BattleProcessManager battleProcessManager;
    private BattleUIManager battleUIManager;

    public override async UniTask LoadingProcess()
    {
        var loadedScene = await AddressableManager.Instance.LoadSceneAsyncWithName(Model.BattleSceneDefine, UnityEngine.SceneManagement.LoadSceneMode.Single);

        if (!loadedScene.IsValid())
            return;

        battleSceneManager = loadedScene.GetRootComponent<BattleSceneManager>();
        
        if (battleSceneManager == null)
        {
            Logger.Null($"BattleBackgroundSceneManager");
            return;
        }

        battleProcessManager = BattleProcessManager.Instance;
        battleUIManager = BattleUIManager.Instance;

        await battleSceneManager.PrepareBattle(Model.DataDungeon);
        await battleUIManager.Prepare();
        await ShowBattleView();
    }

    public override async UniTask Process()
    {
        await battleSceneManager.StartBattle();
        await battleProcessManager.StartBattle(battleSceneManager.LeaderCharacter);
    }

    public override async UniTask Exit()
    {
        await AddressableManager.Instance.UnloadSceneAsync(Model.BattleSceneDefine);
    }

    private async UniTask ShowBattleView()
    {
        BattleViewController battleViewController = new BattleViewController();
        BattleViewModel viewModel = new BattleViewModel();
        battleViewController.SetModel(viewModel);

        await UIManager.Instance.ChangeView(battleViewController, true);
    }
}
