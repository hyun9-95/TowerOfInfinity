using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Battle 관련 매니저들만 참조해야한다!
/// </summary>
public class BattleInfo
{
    #region Property
    public int Level { get; private set; }

    public float BattleExp { get; private set; }

    public float NextBattleExp
    {
        get
        {
            if (expPerLevel == null)
                return 0;

            int nextLevel = Level + 1;
            if (nextLevel >= expPerLevel.Length)
                return expPerLevel[expPerLevel.Length - 1];

            float currentLevelCumulativeExp = Level == 0 ? 0 : expPerLevel[Level - 1];
            float nextLevelCumulativeExp = expPerLevel[Level];

            return nextLevelCumulativeExp - currentLevelCumulativeExp;
        }
    }

    public BattleTeam BattleTeam { get; private set; }

    public CharacterUnit MainCharacter => BattleTeam.CurrentCharacter;

    public CharacterUnitModel MainCharModel => MainCharacter.Model;

    public int KillCount { get; private set; }

    public float BattleStartTime { get; private set; }

    public float ElapsedTime => Time.time - BattleStartTime;

    public int CurrentWave { get; private set; }

    public CharacterDefine FinalBoss { get; private set; }

    public CharacterDefine MidBoss {  get; private set; }

    public DataDungeon DataDungeon { get; private set; }

    public BattleState BattleState { get; private set; }

    public BattleResult BattleResult { get; private set; }
    #endregion

    #region Value
    private float[] expPerLevel;
    #endregion

    public void SetBattleState(BattleState battleState)
    {
        BattleState = battleState;
    }

    public void SetBattleTeam(BattleTeam battleTeam)
    {
        BattleTeam = battleTeam;
        BattleStartTime = Time.time;
        KillCount = 0;
        CurrentWave = 0;
    }

    public void SetDataDungeon(DataDungeon dataDungeon)
    {
        DataDungeon = dataDungeon;
    }

    public void SetBattleResult(BattleResult battleResult)
    {
        BattleResult = battleResult;
    }

    public void SetLevel(int level)
    {
        Level = level;
    }

    public void SetBattleExp(float exp)
    {
        BattleExp = exp;
    }

    public void AddBattleExp(float exp)
    {
        BattleExp += exp;
    }

    public void SetCurrentWave(int wave)
    {
        CurrentWave = wave;
    }

    public void SetExpTable()
    {
        var startExpData = DataManager.Instance.GetDataById<DataBalance>((int)BalanceDefine.BALANCE_BATTLE_START_EXP);
        var expGrowthData = DataManager.Instance.GetDataById<DataBalance>((int)BalanceDefine.BALANCE_BATTLE_EXP_GROWTH_RATE);
        
        var startExp = startExpData.Values[0];
        var expGrowthRate = expGrowthData.Values[0];

        SetExpPerLevel(startExp, expGrowthRate, IntDefine.MAX_BATTLE_LEVEL);
    }

    public void SetExpPerLevel(float startExp, float growthRate, int maxLevel)
    {
        expPerLevel = new float[maxLevel];

        for (int level = 0; level < maxLevel; level++)
            expPerLevel[level] = startExp + (growthRate * level * startExp);
    }

    public void OnExpGain(float exp)
    {
        AddBattleExp(exp);

        while (BattleExp >= NextBattleExp)
        {
            float overflowExp = BattleExp - NextBattleExp;
            Level++;
            BattleExp = overflowExp;
        }
    }

    public void AddKill()
    {
        KillCount++;
    }

}
