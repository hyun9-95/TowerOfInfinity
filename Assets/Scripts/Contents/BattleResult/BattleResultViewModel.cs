using System;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

public class BattleResultViewModel : IBaseViewModel
{
    #region Property
    public bool IsVictory { get; private set; }
    public int KillCount { get; private set; }
    public float ElapsedTime { get; private set; }
    #endregion

    #region Value
    #endregion

    #region Function
    public void SetVictory(bool isVictory)
    {
        IsVictory = isVictory;
    }

    public void SetKillCount(int killCount)
    {
        KillCount = killCount;
    }

    public void SetElapsedTime(float elapsedTime)
    {
        ElapsedTime = elapsedTime;
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