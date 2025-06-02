#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public class BattleSceneManager : BackgroundSceneManager<BattleSceneManager>
{
    #region Property
    public CharacterUnit LeaderCharacter => leaderCharacter;
    #endregion

    #region Value
   
    private BattleEnemyGenerator enemyGenerator;
    private List<CharacterUnit> enemyCharacters = new();
    private List<CharacterUnit> playerCharacters = new();
    private CharacterUnit leaderCharacter;

    // 현재 생존중인 캐릭터들의 모델
    private Dictionary<int, CharacterUnitModel> liveCharacterModelDic = new Dictionary<int, CharacterUnitModel>();
    #endregion

    #region Function
    #region Prepare Battle
    public async UniTask PrepareBattle(DataDungeon dataDungeon)
    {
        CreateEnemyGenerator(dataDungeon);

        if (UseAStar)
            InitializeAStarManager();

        await SpawnPlayerCharacters();
    }

    private void InitializeAStarManager()
    {
        AStarManager.Instance.Initialize(walkableMaps, obstacleMaps, layoutGrid);
    }

    private async UniTask SpawnPlayerCharacters()
    {
        var playerTransform = PlayerStartTransform;
        leaderCharacter = await PlayerManager.Instance.CreateLeaderCharacter(playerTransform);
        leaderCharacter.gameObject.tag = StringDefine.BATTLE_TAG_ALLY;
        leaderCharacter.Initialize();

        await UniTask.NextFrame();

        SetFollowCamera(leaderCharacter.transform);

        playerCharacters.Add(leaderCharacter);
        AddLiveCharacter(leaderCharacter.gameObject.GetInstanceID(), leaderCharacter.Model);
    }

    private void CreateEnemyGenerator(DataDungeon dataDungeon)
    {
        BattleEnemyGeneratorModel enemyGeneratorModel = new BattleEnemyGeneratorModel();
        enemyGeneratorModel.SetDataDungeon(dataDungeon);
        enemyGeneratorModel.SetCurrentWave(0);
        enemyGeneratorModel.SetSpawnInterval(5f);
        enemyGeneratorModel.SetOnSpawnEnemy(OnSpawnEnemy);

        enemyGenerator = new BattleEnemyGenerator(enemyGeneratorModel);
    }

    private void OnSpawnEnemy(CharacterUnit enemy)
    {
        enemy.gameObject.tag = StringDefine.BATTLE_TAG_ENEMY;
        
        var enemyModel = enemy.Model;
        enemyModel.SetTarget(leaderCharacter.Model);
        enemyModel.SetPathFindType(UseAStar ? PathFindType.AStar : PathFindType.Navmesh);

        enemy.Initialize();
        enemy.Activate();

        enemyCharacters.Add(enemy);
        AddLiveCharacter(enemy.gameObject.GetInstanceID(), enemyModel);
    }

    #endregion
    #region Start Battle
    public async UniTask StartBattle()
    {
        await ActiveCharacters();
        await UniTaskUtils.DelaySeconds(FloatDefine.DEFAULT_BATTLE_START_DELAY);

        enemyGenerator.StartGenerateAsync().Forget();
    }

    private async UniTask ActiveCharacters()
    {
        foreach (var character in playerCharacters)
        {
            character.Initialize();
            character.Activate();
        }
    }
    #endregion

    public CharacterUnitModel GetCharacterModel(int instanceId)
    {
        if (!liveCharacterModelDic.ContainsKey(instanceId))
            return null;

        return liveCharacterModelDic[instanceId];
    }

    public void AddLiveCharacter(int instanceId, CharacterUnitModel model)
    {
        liveCharacterModelDic.Add(instanceId, model);
    }

    //죽으면 제거
    public void RemoveLiveCharacter(int instanceId)
    {
        liveCharacterModelDic.Remove(instanceId);
    }
    #endregion

    public void RecalculatePath()
    {
        AStarManager.Instance.RefreshAStarGrid();

        enemyCharacters.ForEach(x => x.Model.ActionHandler.PathFinder.RecalculatePath());
        playerCharacters.ForEach(x => x.Model.ActionHandler.PathFinder.RecalculatePath());

        Logger.Log("Recalculate Path");
    }
}
