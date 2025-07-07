using Cysharp.Threading.Tasks;
using System;

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
        Model.SetLevel(battleInfo.Level);
        Model.SetBattleExp(battleInfo.BattleExp);
        Model.SetNextBattleExp(battleInfo.NextBattleExp);

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
