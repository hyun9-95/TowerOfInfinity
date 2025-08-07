using UnityEngine;

public class BattleCardSelectController : BaseController<BattleCardSelectViewModel>
{
    public override UIType UIType => UIType.BattleCardSelectPopup;

    public override UICanvasType UICanvasType => UICanvasType.Popup;

    private BattleCardSelectPopup View => GetView<BattleCardSelectPopup>();

    public override void Enter()
    {
    }
}
