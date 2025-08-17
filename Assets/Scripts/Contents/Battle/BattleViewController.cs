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
        BattleObserverID.RefreshUI,
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

    public void RefreshBattleInfo(BattleInfo battleInfo)
    {
        Model.SetLevel(battleInfo.Level);
        Model.SetBattleExp(battleInfo.BattleExp);
        Model.SetNextBattleExp(battleInfo.NextBattleExp);
        Model.SetKillCount(battleInfo.KillCount);
        Model.SetCurrentWave(battleInfo.CurrentWave);
        Model.SetBattleStartTime(battleInfo.BattleStartTime);
    }

    void IObserver.HandleMessage(Enum observerMessage, IObserverParam observerParam)
    {
        if (observerParam is not BattleObserverParam)
            return;

        switch (observerMessage)
        {
            case BattleObserverID.RefreshUI:
                View.UpdateUI();
                break;
        }
    }
}
