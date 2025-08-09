#pragma warning disable CS1998
using Cysharp.Threading.Tasks;

public class BattleFlow : BaseFlow<BattleFlowModel>
{
    public override UIType ViewType => UIType.TownView;
    public override FlowType FlowType => FlowType.BattleFlow;

    private BattleSceneManager battleSceneManager;
    private BattleSystemManager battleSystemManager;
    private BattleUIManager battleFXManager;
    private BattleViewController battleViewController;
    private BattleTeam battleTeam;

    public override async UniTask LoadingProcess()
    {
        battleSceneManager = loadedScene.GetRootComponent<BattleSceneManager>();
        
        if (battleSceneManager == null)
        {
            Logger.Null($"BattleBackgroundSceneManager");
            return;
        }

        // BattleSceneManager 하위의 매니저들
        battleSystemManager = BattleSystemManager.Instance;
        battleFXManager = BattleUIManager.Instance;

        battleTeam = await battleSceneManager.CreateBattleTeam(PlayerManager.Instance.MyUser.UserCharacterInfo.CurrentDeck);

        await battleSceneManager.Prepare(Model.DataDungeon);
        await battleSystemManager.Prepare(battleTeam);
        await battleFXManager.Prepare();
    }


    public override async UniTask Process()
    {
        await battleSystemManager.ShowBattleView();
        await battleSceneManager.StartBattle();
    }

    public override async UniTask Exit()
    {
    }
}
