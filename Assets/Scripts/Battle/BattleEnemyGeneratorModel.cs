using System;
using UnityEngine;

public class BattleEnemyGeneratorModel
{
    public BattleEnemyGeneratorModel()
    {
        SpawnIntervalSeconds = FloatDefine.DEFAULT_SPAWN_INTERVAL;
    }

    #region Property
    public float SpawnIntervalSeconds { get; private set; }

    public Action<CharacterUnit> OnSpawnEnemy { get; private set; }

    public bool CheckWalkablePosOnSpawn { get; private set; }

    public CharacterDefine MidBoss { get; private set; }

    public CharacterDefine FinalBoss { get; private set; }

    public int MidBossWave { get; private set; }

    public int FinalBossWave { get; private set; }
    #endregion

    #region Value
    private DataEnemyGroup enemyGroup;
    #endregion

    #region Function
    public void SetDataEnemyGroup(DataEnemyGroup dataEnemyGroup)
    {
        enemyGroup = dataEnemyGroup;

        if (!enemyGroup.IsNullOrEmpty())
        {
            MidBoss = dataEnemyGroup.MidBoss;
            FinalBoss = dataEnemyGroup.FinalBoss;

            MidBossWave = IntDefine.MID_BOSS_WAVE;
            FinalBossWave = IntDefine.FINAL_BOSS_WAVE;
        }
    }

    public void SetSpawnInterval(float interaval)
    {
        SpawnIntervalSeconds = interaval;
    }

    public void SetOnSpawnEnemy(Action<CharacterUnit> func)
    {
        OnSpawnEnemy = func;
    }

    public void SetCheckWalkablePosOnSpawn(bool value)
    {
        CheckWalkablePosOnSpawn = value;
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
