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
    public async UniTask<BattleTeam> CreateBattleTeam(SubCharacterInfo[] currentDeck)
    {
        playerBattleTeam = await CreatePlayerBattleTeam(currentDeck);
        SetCurrentCharacter(playerBattleTeam.CurrentCharacter);

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

    private async UniTask<BattleTeam> CreatePlayerBattleTeam(SubCharacterInfo[] currentDeck)
    {
        await PlayerManager.Instance.UpdateMainPlayerCharacter();

        var playerTransform = PlayerStartTransform;
        var playerCharacters = new List<CharacterUnit>();
        int leaderIndex = 0;

        var mainCharacter = PlayerManager.Instance.GetMainPlayerCharacterUnit();
        mainCharacter.transform.SetPositionAndRotation(playerTransform.position, Quaternion.identity);
        mainCharacter.gameObject.tag = StringDefine.BATTLE_TAG_ALLY;
        playerCharacters.Add(mainCharacter);

        foreach (var subCharacterInfo in currentDeck)
        {
            if (subCharacterInfo == null)
                continue;

            var character = await CharacterFactory.Instance.SpawnSubCharacter(subCharacterInfo, playerTransform);
            character.gameObject.tag = StringDefine.BATTLE_TAG_ALLY;
            character.Initialize();
            
            playerCharacters.Add(character);
        }

        await UniTask.NextFrame();

        var battleTeam = new BattleTeam();
        battleTeam.SetCharacterUnits(playerCharacters);
        battleTeam.SetCurrentIndex(leaderIndex);

        return battleTeam;
    }

    private void SetCurrentCharacter(CharacterUnit leaderCharacter)
    {
        SetFollowCamera(leaderCharacter.transform);
        AddLiveCharacter(leaderCharacter.gameObject.GetInstanceID(), leaderCharacter.Model);
    }

    private void CreateEnemyGenerator(DataDungeon dataDungeon)
    {
        BattleEnemyGeneratorModel enemyGeneratorModel = new BattleEnemyGeneratorModel();
        enemyGeneratorModel.SetDataDungeon(dataDungeon);
        enemyGeneratorModel.SetCurrentWave(0);
        enemyGeneratorModel.SetSpawnInterval(5f);
        enemyGeneratorModel.SetCheckWalkablePosOnSpawn(UseAStar);
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
        await ShowHpBar();
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

    private async UniTask ShowHpBar()
    {
        await BattleUIManager.Instance.ShowHpBar(CurrentCharacter.Model);
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

    public void RefreshAStarGrid()
    {
        AStarManager.Instance.RefreshAStarGrid();
    }
}
