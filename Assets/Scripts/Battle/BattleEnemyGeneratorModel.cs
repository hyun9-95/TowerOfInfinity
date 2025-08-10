using System;
using UnityEngine;

public class BattleEnemyGeneratorModel
{
    public BattleEnemyGeneratorModel()
    {
        enemyGroupContainer = DataManager.Instance.GetDataContainer<DataEnemyGroup>();
        SpawnIntervalSeconds = FloatDefine.DEFAULT_SPAWN_INTERVAL;
        battleStartTime = Time.time;
        CurrentWave = 0;
    }

    #region Property
    public DataDungeon DataDungeon { get; private set; }

    public int CurrentWave { get; private set; }

    public float SpawnIntervalSeconds { get; private set; }

    public Action<CharacterUnit> OnSpawnEnemy { get; private set; }

    public bool CheckWalkablePosOnSpawn { get; private set; }

    public float ElapsedTime => Time.time - battleStartTime;

    public bool IsAllWavesComplete => CurrentWave >= 15;
    #endregion

    #region Value
    private DataContainer<DataEnemyGroup> enemyGroupContainer;
    #endregion

    #region Function
    public void SetDataDungeon(DataDungeon dataDungeon)
    {
        DataDungeon = dataDungeon;
    }

    public void SetCurrentWave(int value)
    {
        CurrentWave = value;
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

    public CharacterDefine[] GetCurrentWave()
    {
        var currentEnemyGroup = DataDungeon.EnemyGroup;
        var enemyGroup = enemyGroupContainer.GetById((int)currentEnemyGroup);

        if (enemyGroup.IsNull)
            return null;

        if (enemyGroup.EnemysCount < CurrentWave)
            return enemyGroup.GetEnemys(CurrentWave);

        // 모든 웨이브가 끝난 경우에 보스 스폰
        return new CharacterDefine[] { enemyGroup.Boss };
    }
    #endregion
}
