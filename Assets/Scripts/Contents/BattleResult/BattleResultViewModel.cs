using System;
using UnityEngine;

public class BattleResultViewModel : IBaseViewModel
{
    #region Property
    public BattleResult BattleResult { get; private set; }
    public int KillCount { get; private set; }
    public float ElapsedTime { get; private set; }
    public Action OnReturnToTown { get; private set; }
    #endregion

    #region Value
    #endregion

    #region Function
    public void SetByBattleInfo(BattleInfo battleInfo)
    {
        BattleResult = battleInfo.BattleResult;
        KillCount = battleInfo.KillCount;
        ElapsedTime = battleInfo.ElapsedTime;
    }

    public void SetOnReturnToTown(Action onReturn)
    {
        OnReturnToTown = onReturn;
    }

    public string GetKillCountText()
    {
        var sb = GlobalStringBuilder.Get();
        sb.Append(LocalizationManager.GetLocalization(LocalizationDefine.LOCAL_WORD_KILL_COUNT));
        sb.Append(" - ");
        sb.Append(KillCount.ToString());

        return sb.ToString();
    }

    public string GetElapsedTimeText()
    {
        var sb = GlobalStringBuilder.Get();

        int minutes = Mathf.FloorToInt(ElapsedTime / FloatDefine.MINUTE_TO_SECONDS);
        int seconds = Mathf.FloorToInt(ElapsedTime % FloatDefine.MINUTE_TO_SECONDS);

        sb.Append(LocalizationManager.GetLocalization(LocalizationDefine.LOCAL_WORD_TIME));
        sb.Append(" - ");
        sb.Append($"{minutes:00}:{seconds:00}");

        return sb.ToString();
    }
    #endregion
}