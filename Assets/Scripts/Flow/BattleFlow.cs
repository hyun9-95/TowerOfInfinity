#pragma warning disable CS1998
using Cysharp.Threading.Tasks;

public class BattleFlow : BaseFlow<BattleFlowModel>
{
    public override UIType ViewType => UIType.LobbyView;
    public override FlowType FlowType => FlowType.BattleFlow;

    private BattleSceneManager battleSceneManager;
    private BattleSystemManager battleSystemManager;
    private BattleFXManager battleFXManager;
    private BattleViewController battleViewController;
    private BattleTeam battleTeam;

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

        // BattleSceneManager 하위의 매니저들
        battleSystemManager = BattleSystemManager.Instance;
        battleFXManager = BattleFXManager.Instance;

        battleTeam = await battleSceneManager.CreateBattleTeam(PlayerManager.Instance.MyUser.UserTeams);
        await battleSceneManager.PrepareBattle(Model.DataDungeon);
        await battleFXManager.Prepare();
        await ShowBattleView();
    }

    private async UniTask ShowBattleView()
    {
        battleViewController = new BattleViewController();
        BattleViewModel viewModel = new BattleViewModel();
        battleViewController.SetModel(viewModel);

        await UIManager.Instance.ChangeView(battleViewController, true);
    }

    public override async UniTask Process()
    {
        await battleSceneManager.StartBattle();
        await battleSystemManager.StartBattle(battleTeam, battleViewController);
    }

    public override async UniTask Exit()
    {
        await AddressableManager.Instance.UnloadSceneAsync(Model.BattleSceneDefine);
    }
}
