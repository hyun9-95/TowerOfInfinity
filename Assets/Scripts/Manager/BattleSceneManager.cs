#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public class BattleSceneManager : BackgroundSceneManager<BattleSceneManager>
{
    #region Property
    public CharacterUnit CurrentCharacter => playerBattleTeam.CurrentCharacter;
    #endregion

    #region Value
   
    private BattleEnemyGenerator enemyGenerator;
    private List<CharacterUnit> enemyCharacters = new();
    private BattleTeam playerBattleTeam = new ();

    // 현재 생존중인 캐릭터들의 모델
    private Dictionary<int, CharacterUnitModel> liveCharacterModelDic = new Dictionary<int, CharacterUnitModel>();
    #endregion

    #region Function
    #region Prepare Battle
    public async UniTask<BattleTeam> CreateBattleTeam(UserCharacter[] userTeams)
    {
        playerBattleTeam = await CreatePlayerBattleTeam(userTeams);

        return playerBattleTeam;
    }

    public async UniTask PrepareBattle(DataDungeon dataDungeon)
    {
        CreateEnemyGenerator(dataDungeon);

        if (UseAStar)
            InitializeAStarManager();
    }

    private void InitializeAStarManager()
    {
        AStarManager.Instance.Initialize(walkableMaps, obstacleMaps, layoutGrid);
    }

    private async UniTask<BattleTeam> CreatePlayerBattleTeam(UserCharacter[] userTeams)
    {
        var playerTransform = PlayerStartTransform;
        var playerCharacters = new List<CharacterUnit>();
        int leaderIndex = 0;

        foreach (var userCharacter in userTeams)
        {
            if (userCharacter == null)
                continue;

            var character = await CharacterFactory.Instance.CreateCharacter(playerTransform, userCharacter);
            character.gameObject.tag = StringDefine.BATTLE_TAG_ALLY;
            character.Initialize();
            
            playerCharacters.Add(character);
        }

        await UniTask.NextFrame();

        var leaderCharacter = playerBattleTeam.CurrentCharacter;

        SetFollowCamera(leaderCharacter.transform);
        AddLiveCharacter(leaderCharacter.gameObject.GetInstanceID(), leaderCharacter.Model);

        var battleTeam = new BattleTeam();
        battleTeam.SetCharacterUnits(playerCharacters);
        battleTeam.SetCurrentIndex(leaderIndex);

        return battleTeam;
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
        enemyModel.SetTarget(playerBattleTeam.CurrentCharacter.Model);
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
        foreach (var character in playerBattleTeam.CharacterUnits)
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
        playerBattleTeam.CharacterUnits.ForEach(x => x.Model.ActionHandler.PathFinder.RecalculatePath());

        Logger.Log("Recalculate Path");
    }
}
