using System;
using UnityEngine;

public class BattleViewModel : IBaseViewModel
{
    #region Property
    public int Level { get; private set; }
    public float BattleExp { get; private set; }
    public float CurrentLevelExp { get; private set; }
    public float NextBattleExp { get; private set; }
    public int KillCount { get; private set; }
    public float BattleStartTime { get; private set; }
    public int CurrentWave { get; private set; }
    #endregion

    #region Value
    public void SetLevel(int level)
    {
        Level = level;
    }

    public void SetBattleExp(float exp)
    {
        BattleExp = exp;
    }

    public void SetCurrentLevelExp(float exp)
    {
        CurrentLevelExp = exp;
    }

    public void SetNextBattleExp(float exp)
    {
        NextBattleExp = exp;
    }

    public void SetKillCount(int killCount)
    {
        KillCount = killCount;
    }

    public void SetBattleStartTime(float startTime)
    {
        BattleStartTime = startTime;
    }

    public void SetCurrentWave(int currentWave)
    {
        CurrentWave = currentWave;
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

        float elapsedTime = Time.time - BattleStartTime;
        int minutes = Mathf.FloorToInt(elapsedTime / FloatDefine.MINUTE_TO_SECONDS);
        int seconds = Mathf.FloorToInt(elapsedTime % FloatDefine.MINUTE_TO_SECONDS);

        sb.Append(LocalizationManager.GetLocalization(LocalizationDefine.LOCAL_WORD_TIME));
        sb.Append(" - ");
        sb.Append($"{minutes:00}:{seconds:00}");

        return sb.ToString();
    }

    public string GetWaveText()
    {
        var sb = GlobalStringBuilder.Get();
        sb.Append(LocalizationManager.GetLocalization(LocalizationDefine.LOCAL_WORD_WAVE));
        sb.Append(" - ");
        sb.Append((CurrentWave + 1).ToString());

        return sb.ToString();
    }

    public float GetExpSliderValue()
    {
        if (NextBattleExp <= CurrentLevelExp)
        {
            Logger.Warning($"NextBattleExp ({NextBattleExp}) <= CurrentLevelExp ({CurrentLevelExp})");
            return 1;
        }

        float expRange = NextBattleExp - CurrentLevelExp;
        if (expRange <= 0)
        {
            Logger.Warning($"ExpRange <= 0: {expRange}");
            return 1;
        }

        float currentProgress = BattleExp - CurrentLevelExp;
        float sliderValue = Mathf.Clamp01(currentProgress / expRange);

        Logger.Log($"Exp Slider: Level={Level}, BattleExp={BattleExp:F1}, CurrentLevelExp={CurrentLevelExp:F1}, NextBattleExp={NextBattleExp:F1}, Progress={currentProgress:F1}/{expRange:F1}, Value={sliderValue:F3}");

        if (currentProgress < 0)
        {
            Logger.Error($"Current progress is negative: {currentProgress}");
            return 0;
        }

        return sliderValue;
    }
    #endregion
}
