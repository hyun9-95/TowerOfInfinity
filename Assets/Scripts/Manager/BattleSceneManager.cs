#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 전투 씬 초기 세팅 및, 적과 아군의 스폰을 담당한다.
/// </summary>
public class BattleSceneManager : BackgroundSceneManager<BattleSceneManager>
{
    #region Property
    #endregion

    #region Value
    [SerializeField]
    private BattleInfinityTile infinityTile;

    [SerializeField]
    private ScriptableEnemyWeightInfo enemyWeightInfo;

    private BattleEnemySpawner enemySpawn;
    private List<CharacterUnit> enemyCharacters = new();
    private BattleInfo battleInfo;

    // 현재 생존중인 캐릭터들의 모델
    private Dictionary<int, CharacterUnitModel> liveCharacterModelDic = new Dictionary<int, CharacterUnitModel>();
    #endregion

    #region Function
    #region Prepare Battle
    public async UniTask Prepare(BattleInfo battleInfo)
    {
        this.battleInfo = battleInfo;

        SetCurrentCharacter(battleInfo.MainCharacter);
        CreateEnemyGenerator(battleInfo.DataDungeon);

        await infinityTile.Prepare(battleInfo.MainCharacter.transform);
    }


    private void SetCurrentCharacter(CharacterUnit leaderCharacter)
    {
        SetFollowCamera(leaderCharacter.transform);
        AddLiveCharacter(leaderCharacter.gameObject.GetInstanceID(), leaderCharacter.Model);
    }

    private void CreateEnemyGenerator(DataDungeon dataDungeon)
    {
        var dataEnemyGroup = DataManager.Instance.GetDataById<DataEnemyGroup>((int)dataDungeon.EnemyGroup);

        BattleEnemyGeneratorModel enemyGeneratorModel = new BattleEnemyGeneratorModel();
        enemyGeneratorModel.SetDataEnemyGroup(dataEnemyGroup);
        enemyGeneratorModel.SetCheckWalkablePosOnSpawn(true);
        enemyGeneratorModel.SetOnSpawnEnemy(OnSpawnEnemy);
        enemyGeneratorModel.SetOnGetEnemySpawnWeight(OnGetEnemySpawnWeight);

        enemySpawn = new BattleEnemySpawner(enemyGeneratorModel);
    }

    private void OnSpawnEnemy(CharacterUnit enemy)
    {
        var enemyModel = enemy.Model;
        enemyModel.SetTarget(battleInfo.MainCharacter.Model);
        enemyModel.SetPathFindType(PathFindType.AStar);

        enemy.Initialize();
        enemy.Activate();

        enemyCharacters.Add(enemy);
        AddLiveCharacter(enemy.gameObject.GetInstanceID(), enemyModel);
    }

    private float OnGetEnemySpawnWeight(CharacterDefine enemy, int currentWave)
    {
        if (enemyWeightInfo == null)
        {
            Logger.Null("Enemy Weight Info");
            return 0f;
        }

        // 현재 웨이브에 해당하는 적의 가중치를 반환
        return enemyWeightInfo.GetCurrentWeight(enemy, currentWave);
    }
    #endregion

    public async UniTask StartSpawn()
    {
        enemySpawn.StartGenerateAsync().Forget();
    }

    public static CharacterUnitModel GetCharacterModel(int instanceId)
    {
        if (instance == null)
            return null;

        var liveCharacterModelDic = instance.liveCharacterModelDic;

        if (!liveCharacterModelDic.ContainsKey(instanceId))
            return null;

        return liveCharacterModelDic[instanceId];
    }

    public static CharacterUnitModel GetCharacterModel(Transform transform)
    {
        return GetCharacterModel(transform.gameObject.GetInstanceID());
    }

    public static CharacterUnitModel GetCharacterModel(GameObject gameObject)
    {
        return GetCharacterModel(gameObject.GetInstanceID());
    }

    public static CharacterUnitModel GetCharacterModel(Collider2D collider)
    {
        return GetCharacterModel(collider.gameObject.GetInstanceID());
    }

    private void AddLiveCharacter(int instanceId, CharacterUnitModel model)
    {
        liveCharacterModelDic.Add(instanceId, model);
    }

    //죽으면 제거
    public static void RemoveLiveCharacter(int instanceId)
    {
        if (instance == null)
            return;

        var liveCharacterModelDic = instance.liveCharacterModelDic;

        liveCharacterModelDic.Remove(instanceId);
    }

    #endregion
    public void StopSpawn()
    {
        enemySpawn.Cancel();
    }

#if CHEAT
    public void CheatSpawnBoss()
    {
        enemySpawn.CheatSpawnBoss();
    }
#endif
}
