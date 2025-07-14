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
        await battleSystemManager.Prepare(battleTeam);
        await battleFXManager.Prepare();

        await ShowBattleView(battleSystemManager.BattleInfo);
    }

    private async UniTask ShowBattleView(BattleInfo battleInfo)
    {
        battleViewController = new BattleViewController();
        BattleViewModel viewModel = new BattleViewModel();

        // 입력이 필요한 이벤트를 바인딩한다. (전투 로직은 battleSystemManager 전담)
        battleSystemManager.BindingBattleViewEvent(viewModel);

        viewModel.SetByBattleInfo(battleInfo);

        battleViewController.SetModel(viewModel);

        await UIManager.Instance.ChangeView(battleViewController, true);
    }

    public override async UniTask Process()
    {
        await battleSceneManager.StartBattle();
    }

    public override async UniTask Exit()
    {
        await AddressableManager.Instance.UnloadSceneAsync(Model.BattleSceneDefine);
    }
}
