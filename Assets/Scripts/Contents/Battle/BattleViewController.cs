using UnityEngine;

public class BattleViewController : BaseController<BattleViewModel>
{
    public override UIType UIType => UIType.BattleView;

    public override UICanvasType UICanvasType => UICanvasType.View;

    private BattleView View => GetView<BattleView>();

    public override void Enter()
    {
	}
}
