using System;
using UnityEngine;

public class BattleEnemyGeneratorModel
{
    public BattleEnemyGeneratorModel()
    {
        enemyGroupContainer = DataManager.Instance.GetDataContainer<DataEnemyGroup>();
        SpawnIntervalSeconds = FloatDefine.DEFAULT_SPAWN_INTERVAL;
    }

    #region Property
    public DataDungeon DataDungeon { get; private set; }

    public int CurrentWave { get; private set; }

    public float SpawnIntervalSeconds { get; private set; }

    public Action<CharacterUnit> OnSpawnEnemy { get; private set; }
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

    public CharacterDefine[] GetCurrentWave()
    {
        var currentEnemyGroup = DataDungeon.EnemyGroups[CurrentWave];
        var enemyGroup = enemyGroupContainer.GetById((int)currentEnemyGroup);

        if (enemyGroup.IsNull)
            return null;

        return enemyGroup.Enemys;
    }
    #endregion
}
