using System;

public class BattleViewModel : IBaseViewModel
{
    #region Property
    public int Level { get; private set; }
    public float BattleExp { get; private set; }
    public float NextBattleExp { get; private set; }
    public int KillCount { get; private set; }
    public float ElapsedTime { get; private set; }
    public int CurrentWave { get; private set; }
    #endregion

    #region Value

    public void SetByBattleInfo(BattleInfo battleInfo)
    {
        SetLevel(battleInfo.Level);
        SetBattleExp(battleInfo.BattleExp);
        SetNextBattleExp(battleInfo.NextBattleExp);
        SetKillCount(battleInfo.KillCount);
        SetElapsedTime(battleInfo.ElapsedTime);
        SetCurrentWave(battleInfo.CurrentWave);
    }

    public void SetLevel(int level)
    {
        Level = level;
    }

    public void SetBattleExp(float exp)
    {
        BattleExp = exp;
    }

    public void SetNextBattleExp(float exp)
    {
        NextBattleExp = exp;
    }

    public void SetKillCount(int killCount)
    {
        KillCount = killCount;
    }

    public void SetElapsedTime(float elapsedTime)
    {
        ElapsedTime = elapsedTime;
    }

    public void SetCurrentWave(int currentWave)
    {
        CurrentWave = currentWave;
    }
    #endregion
}
