public class BattleResultController : BaseController<BattleResultViewModel>
{
    public override UIType UIType => UIType.BattleResultPopup;

    public override UICanvasType UICanvasType => UICanvasType.Popup;

    private BattleResultView View => GetView<BattleResultView>();

    public override void Enter()
    {
    }
}