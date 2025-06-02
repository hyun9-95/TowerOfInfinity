using UnityEngine;

public class LobbyController : BaseController<LobbyViewModel>
{
    public override UIType UIType => UIType.LobbyView;

    public override UICanvasType UICanvasType => UICanvasType.View;

    private LobbyView View => GetView<LobbyView>();

    public override void Enter()
    {
	}
}
