using System;

public class BattleResultViewModel : IBaseViewModel
{
    #region Property
    public bool IsVictory { get; private set; }
    public int KillCount { get; private set; }
    public float ElapsedTime { get; private set; }
    #endregion

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
}