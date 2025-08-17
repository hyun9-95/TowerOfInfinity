#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// 전투의 기본 UI 및 입력을 관리한다.
/// </summary>
public class BattleViewController : BaseController<BattleViewModel>
{
    public override UIType UIType => UIType.BattleView;

    public override UICanvasType UICanvasType => UICanvasType.View;

    private BattleView View => GetView<BattleView>();

    private CancellationTokenSource cts;

    public void RefreshBattleInfo(BattleInfo battleInfo)
    {
        Model.SetLevel(battleInfo.Level);
        Model.SetBattleExp(battleInfo.BattleExp);
        Model.SetNextBattleExp(battleInfo.NextBattleExp);
        Model.SetKillCount(battleInfo.KillCount);
        Model.SetCurrentWave(battleInfo.CurrentWave);
        Model.SetBattleStartTime(battleInfo.BattleStartTime);
        Model.SetBattleState(battleInfo.BattleState);

        if (battleInfo.BattleState == BattleState.End)
            cts.Cancel();
    }

    public override async UniTask Process()
    {
        View.UpdateUI();
        ShowBattleUIAsync().Forget();
    }

    private async UniTask ShowBattleUIAsync()
    {
        cts = new CancellationTokenSource();

        while (!cts.Token.IsCancellationRequested)
        {
            if (View)
                View.UpdateUI();

            await UniTaskUtils.WaitForLastUpdate(cts.Token); 
        }
    }
}
