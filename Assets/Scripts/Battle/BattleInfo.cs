using UnityEngine;

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

            return expPerLevel[nextLevel];
        }
    }

    public BattleTeam BattleTeam { get; private set; }

    public CharacterUnit CurrentCharacter => BattleTeam.CurrentCharacter;

    public int KillCount { get; private set; }

    public float BattleStartTime { get; private set; }

    public float ElapsedTime => Time.time - BattleStartTime;

    public int CurrentWave { get; private set; }

    public CharacterDefine BossCharacterDefine { get; private set; }

    public DataDungeon DataDungeon { get; private set; }

    public BattleState BattleState { get; private set; }
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
        
        var enemyGroup = DataManager.Instance.GetDataById<DataEnemyGroup>((int)dataDungeon.EnemyGroup);
        
        if (!enemyGroup.IsNull)
            BossCharacterDefine = enemyGroup.Boss;
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
            expPerLevel[level] = startExp + (growthRate * level);
    }

    public void OnExpGain(float exp)
    {
        AddBattleExp(exp);

        if (BattleExp >= NextBattleExp)
        {
            Level++;
            BattleExp = 0;
        }
    }

    public void AddKill()
    {
        KillCount++;
    }

}
