#pragma warning disable CS1998
using Cysharp.Threading.Tasks;

public class BattleCardSelectController : BaseController<BattleCardSelectViewModel>
{
    public override UIType UIType => UIType.BattleCardSelectPopup;

    public override UICanvasType UICanvasType => UICanvasType.Popup;

    private BattleCardSelectPopup View => GetView<BattleCardSelectPopup>();

    public override void Enter()
    {
        Model.SetOnClickBattleCard(OnSelectBattleCard);
    }

    public void OnSelectBattleCard(int index)
    {
        if (index >= Model.CardUnitModelList.Count)
            return;

        var cardUnitModel = Model.CardUnitModelList[index];

        Model.OnSelectBattleCard(cardUnitModel.CardData);

        UIManager.Instance.Back().Forget();
    }

    public override async UniTask Exit()
    {
        await base.Exit();
        Model.OnCompleteSelect();
    }
}
