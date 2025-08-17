using Cysharp.Threading.Tasks;
using System;

/// <summary>
/// 전투의 기본 UI 및 입력을 관리한다.
/// </summary>
public class BattleViewController : BaseController<BattleViewModel>
{
    public override UIType UIType => UIType.BattleView;

    public override UICanvasType UICanvasType => UICanvasType.View;

    private BattleView View => GetView<BattleView>();

    public void RefreshBattleInfo(BattleInfo battleInfo)
    {
        Model.SetLevel(battleInfo.Level);
        Model.SetBattleExp(battleInfo.BattleExp);
        Model.SetNextBattleExp(battleInfo.NextBattleExp);
        Model.SetKillCount(battleInfo.KillCount);
        Model.SetCurrentWave(battleInfo.CurrentWave);
        Model.SetBattleStartTime(battleInfo.BattleStartTime);
    }
}
