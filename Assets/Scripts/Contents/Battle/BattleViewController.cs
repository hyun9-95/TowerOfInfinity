#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using System;
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

    public void RefreshBattleInfo(BattleInfoParam battleInfoParam)
    {
        Model.SetLevel(battleInfoParam.Level);
        Model.SetBattleExp(battleInfoParam.BattleExp);
        Model.SetNextBattleExp(battleInfoParam.NextBattleExp);
        Model.SetKillCount(battleInfoParam.KillCount);
        Model.SetCurrentWave(battleInfoParam.CurrentWave);
        Model.SetBattleStartTime(battleInfoParam.BattleStartTime);
        Model.SetBattleState(battleInfoParam.BattleState);

        if (battleInfoParam.BattleState == BattleState.End)
            cts.Cancel();

        if (battleInfoParam.AbilitySlotDic != null)
        {
            if (Model.AbilitySlotUnitModel == null)
                Model.SetAbilitySlotUnitModel(new AbilitySlotUnitModel());

            Model.AbilitySlotUnitModel.SyncWithAbilityProcessor(battleInfoParam.AbilitySlotDic);
            View.ShowAblitySlotUnit();
        }
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
