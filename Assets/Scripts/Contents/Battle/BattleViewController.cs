using Cysharp.Threading.Tasks;
using System;

/// <summary>
/// 전투의 기본 UI 및 입력을 관리한다.
/// </summary>
public class BattleViewController : BaseController<BattleViewModel>, IObserver
{
    public override UIType UIType => UIType.BattleView;

    public override UICanvasType UICanvasType => UICanvasType.View;

    private BattleView View => GetView<BattleView>();

    private BattleObserverID[] observerIDs = new BattleObserverID[]
    {
        BattleObserverID.ExpGain,
    };

    public override void Enter()
    {
        ObserverManager.AddObserver(observerIDs, this);
    }

    public override async UniTask Exit()
    {
        ObserverManager.RemoveObserver(observerIDs, this);
        await base.Exit();
    }

    private void Refresh(BattleInfo battleInfo)
    {
        Model.SetByBattleInfo(battleInfo);

        View.ShowAsync().Forget();
    }

    void IObserver.HandleMessage(Enum observerMessage, IObserverParam observerParam)
    {
        if (observerParam is not BattleObserverParam)
            return;

        BattleObserverParam param = (BattleObserverParam)observerParam;

        switch (observerMessage)
        {
            case BattleObserverID.ExpGain:
                Refresh(param.BattleInfo);
                break;
        }
    }
}
