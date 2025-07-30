using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class TownViewController : BaseController<TownViewModel>
{
    public override UIType UIType => UIType.TownView;

    public override UICanvasType UICanvasType => UICanvasType.View;

    private TownView View => GetView<TownView>();

    public override void Enter()
    {
	}

    private void OnClickCustomization()
    {
    }
}
