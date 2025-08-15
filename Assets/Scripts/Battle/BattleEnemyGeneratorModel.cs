using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleEnemyGeneratorModel
{
    #region Property
    public float SpawnIntervalSeconds { get; private set; }

    public float SpawnIntervalDecrease { get; private set; }

    public Action<CharacterUnit> OnSpawnEnemy { get; private set; }

    public bool CheckWalkablePosOnSpawn { get; private set; }

    public CharacterDefine MidBoss { get; private set; }

    public CharacterDefine FinalBoss { get; private set; }

    public int MidBossWave { get; private set; }

    public int FinalBossWave { get; private set; }

    public Func<CharacterDefine, int, float> OnGetEnemySpawnWeight { get; private set; }
    #endregion

    #region Value
    private DataEnemyGroup enemyGroup;
    private Dictionary<int, int> burstWaveInfoDic = new();
    #endregion

    #region Function
    public void SetDataEnemyGroup(DataEnemyGroup dataEnemyGroup)
    {
        if (dataEnemyGroup.IsNullOrEmpty())
            return;

        enemyGroup = dataEnemyGroup;

        MidBoss = dataEnemyGroup.MidBoss;
        FinalBoss = dataEnemyGroup.FinalBoss;

        MidBossWave = IntDefine.MID_BOSS_WAVE;
        FinalBossWave = IntDefine.FINAL_BOSS_WAVE;

        SpawnIntervalSeconds = dataEnemyGroup.SpawnInterval;
        SpawnIntervalDecrease = dataEnemyGroup.IntervalDecrease;

        if (dataEnemyGroup.BurstWave != null && dataEnemyGroup.BurstValue != null)
        {
            if (dataEnemyGroup.BurstValue.Length != dataEnemyGroup.BurstWave.Length)
                return;

            for (int i = 0; i < dataEnemyGroup.BurstWave.Length; i++)
            {
                var wave = dataEnemyGroup.BurstWave[i];
                var value = dataEnemyGroup.BurstValue[i];

                burstWaveInfoDic[wave] = value;
            }
        }
    }

    public void SetOnSpawnEnemy(Action<CharacterUnit> func)
    {
        OnSpawnEnemy = func;
    }

    public void SetOnGetEnemySpawnWeight(Func<CharacterDefine, int, float> func)
    {
        OnGetEnemySpawnWeight = func;
    }

    public void SetCheckWalkablePosOnSpawn(bool value)
    {
        CheckWalkablePosOnSpawn = value;
    }

    public int GetBurstWaveValue(int currentWave)
    {
        if (burstWaveInfoDic.TryGetValue(currentWave, out int value))
            return value;

        return 0;
    }

    public float GetCurrentSpawnInterval(int currentWave)
    {
        if (SpawnIntervalSeconds <= 0f)
            return 0f;

        return Mathf.Max(SpawnIntervalSeconds - (SpawnIntervalDecrease * currentWave), 0f);
    }

    public CharacterDefine[] GetCurrentWaveEnemies(int currentWave)
    {
        if (enemyGroup.IsNullOrEmpty())
            return null;

        if (currentWave < IntDefine.MAX_DUNGEON_WAVE_COUNT)
            return enemyGroup.GetEnemys(currentWave);

        return null;
    }
    #endregion
}
