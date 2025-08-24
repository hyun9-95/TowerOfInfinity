#pragma warning disable CS1998
using Cysharp.Threading.Tasks;

public class IntroFlow : BaseFlow<IntroFlowModel>
{
    public override UIType ViewType => UIType.TownView;

    public override FlowType FlowType => FlowType.IntroFlow;

    public override async UniTask LoadingProcess()
    {
        await AddressableManager.Instance.InitializeAsync();
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
#if UNITY_EDITOR && CHEAT
        CheatEnterGame();
#else
        EnterGame();
#endif
    }

    private void EnterGame()
    {
        if (PlayerManager.Instance.User.IsCompletePrologue)
        {
            FlowManager.Instance.ChangeCurrentTownFlow().Forget();
        }
        else
        {
            var customizationFlowModel = new CustomizationFlowModel();
            FlowManager.Instance.ChangeFlow(FlowType.CustomizationFlow, customizationFlowModel).Forget();
        }
    }

    private void CheatEnterGame()
    {
#if UNITY_EDITOR && CHEAT
        if (CheatManager.CheatConfig.IsEnterBattleDirectly)
        {
            BattleFlowModel battleFlowModel = new BattleFlowModel();
            battleFlowModel.SetDataDungeon(DataManager.Instance.GetDataById<DataDungeon>((int)DungeonDefine.DUNGEON_RUINS));

            FlowManager.Instance.ChangeFlow(FlowType.BattleFlow, battleFlowModel).Forget();
            return;
        }
        else
        {
            EnterGame();
        }
#endif
    }

    public override async UniTask Exit()
    {
    }
}
