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

        SetCurrentCharacter(battleInfo.CurrentCharacter);
        CreateEnemyGenerator(battleInfo.DataDungeon);

        if (UseAStar)
            InitializeAStarManager();
    }

    private void InitializeAStarManager()
    {
        AStarManager.Instance.Initialize(walkableMaps, obstacleMaps, layoutGrid);
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
        enemyGeneratorModel.SetSpawnInterval(5f);
        enemyGeneratorModel.SetCheckWalkablePosOnSpawn(UseAStar);
        enemyGeneratorModel.SetOnSpawnEnemy(OnSpawnEnemy);

        enemySpawn = new BattleEnemySpawner(enemyGeneratorModel);
    }

    private void OnSpawnEnemy(CharacterUnit enemy)
    {
        enemy.gameObject.tag = StringDefine.BATTLE_TAG_ENEMY;
        
        var enemyModel = enemy.Model;
        enemyModel.SetTarget(battleInfo.CurrentCharacter.Model);
        enemyModel.SetPathFindType(UseAStar ? PathFindType.AStar : PathFindType.Navmesh);

        enemy.Initialize();
        enemy.Activate();

        enemyCharacters.Add(enemy);
        AddLiveCharacter(enemy.gameObject.GetInstanceID(), enemyModel);
    }
    #endregion

    public async UniTask StartSpawn()
    {
        enemySpawn.StartGenerateAsync().Forget();
    }

    public CharacterUnitModel GetCharacterModel(int instanceId)
    {
        if (!liveCharacterModelDic.ContainsKey(instanceId))
            return null;

        return liveCharacterModelDic[instanceId];
    }

    public CharacterUnitModel GetCharacterModel(Collider2D collider)
    {
        return GetCharacterModel(collider.gameObject.GetInstanceID());
    }

    private void AddLiveCharacter(int instanceId, CharacterUnitModel model)
    {
        liveCharacterModelDic.Add(instanceId, model);
    }

    //죽으면 제거
    public void RemoveLiveCharacter(int instanceId)
    {
        liveCharacterModelDic.Remove(instanceId);
    }
    #endregion

    public void RefreshAStarGrid()
    {
        AStarManager.Instance.RefreshAStarGrid();
    }

    public void StopSpawn()
    {
        enemySpawn.Cancel();
    }

#if CHEAT
    public void CheatSpawnBoss()
    {
        if (!BattleSystemManager.Instance.InBattle)
            return;

        enemySpawn.CheatSpawnBoss();
    }
#endif
}
