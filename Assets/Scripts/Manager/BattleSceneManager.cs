#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

/// <summary>
/// 전투 씬 초기 세팅 및, 적과 아군의 스폰을 담당한다.
/// </summary>
public class BattleSceneManager : BackgroundSceneManager<BattleSceneManager>, IObserver
{
    #region Property
    #endregion

    #region Value
    [SerializeField]
    private BattleInfinityTile infinityTile;

    [SerializeField]
    private BattleSystem battleSystem;

    [SerializeField]
    private BattleWorldUI battleWorldUI;

    [SerializeField]
    private TargetVirtualCameraBlending bossBlendingCamera;

    [SerializeField]
    private ScriptableEnemyWeightInfo enemyWeightInfo;

    private BattleEnemySpawner enemySpawn;
    private BattleApiModel battleApiModel;
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

        await battleSystem.Prepare(battleInfo);
        await infinityTile.Prepare(battleInfo.MainCharacter.transform);
        await battleWorldUI.Prepare();

        PrepareBattleApi();

        ObserverManager.AddObserver(BattleObserverID.DeadCharacter, this);
    }

    private void PrepareBattleApi()
    {
        battleApiModel = new BattleApiModel();

        // BattleScene
        battleApiModel.SetOnGetAliveCharModelById(OnGetAliveCharModel);

        // BattleSystem
        battleApiModel.SetOnDamage(battleSystem.OnDamage);
        battleApiModel.SetOnHeal(battleSystem.OnHeal);
        battleApiModel.SetOnExpGainRangeUp(battleSystem.OnExpGainRangeUp);

#if CHEAT
        battleApiModel.SetOnCheatSpawnBoss(CheatSpawnBoss);
        battleApiModel.SetOnCheatLevelUpWithDraw(battleSystem.CheatLevelUpWithDraw);
        battleApiModel.SetOnCheatLevelUp(battleSystem.CheatLevelUp);
        battleApiModel.SetOnCheatAddWave(battleSystem.CheatAddWave);
#endif

        BattleApi.InitializeModel(battleApiModel);
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

        if (enemyModel.CharacterType == CharacterType.Boss)
        {
            BossBlendingAsync(enemy).Forget();
        }
        else
        {
            enemy.Initialize();
            enemy.Activate();

            enemyCharacters.Add(enemy);
            AddLiveCharacter(enemy.gameObject.GetInstanceID(), enemyModel);
        }
    }

    private async UniTask BossBlendingAsync(CharacterUnit enemy)
    {
        InputManager.EnableMoveInput(false);
        InputManager.EnableActionButtons(false);

        enemy.Initialize();

        var animator = enemy.GetComponent<Animator>();
        animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        animator.SetInteger(StringDefine.CHARACTER_ANIM_STATE_KEY, 0);

        bossBlendingCamera.SetTarget(enemy.transform);

        Time.timeScale = 0;

        await bossBlendingCamera.StartBlending(2f, false);

        Time.timeScale = 1;

        animator.updateMode = AnimatorUpdateMode.Normal;

        InputManager.EnableMoveInput(true);
        InputManager.EnableActionButtons(true);

        enemy.Activate();

        enemyCharacters.Add(enemy);
        AddLiveCharacter(enemy.gameObject.GetInstanceID(), enemy.Model);
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

    public async UniTask StartBattle()
    {
        await battleSystem.StartBattle();
        await battleWorldUI.ShowHpBar(battleInfo.MainCharModel);

        enemySpawn.StartGenerateAsync().Forget();
    }

    public static CharacterUnitModel OnGetAliveCharModel(int instanceId)
    {
        if (Instance == null)
            return null;

        var liveCharacterModelDic = Instance.liveCharacterModelDic;

        if (!liveCharacterModelDic.ContainsKey(instanceId))
            return null;

        return liveCharacterModelDic[instanceId];
    }

    private void AddLiveCharacter(int instanceId, CharacterUnitModel model)
    {
        liveCharacterModelDic.Add(instanceId, model);
    }

    //죽으면 제거
    private void RemoveLiveCharacter(int instanceId)
    {
        liveCharacterModelDic.Remove(instanceId);
    }

    public void Stop()
    {
        battleSystem.Stop();
        enemySpawn.Cancel();
        ObserverManager.RemoveObserver(BattleObserverID.DeadCharacter, this);
    }

    void IObserver.HandleMessage(Enum observerMessage, IObserverParam observerParam)
    {
        if (observerParam is not BattleObserverParam battleObserverParam)
            return;

        switch (observerMessage)
        {
            case BattleObserverID.DeadCharacter:
                RemoveLiveCharacter(battleObserverParam.IntValue);
                break;
        }
    }

    #endregion

#if CHEAT
    public void CheatSpawnBoss()
    {
        enemySpawn.CheatSpawnBoss();
    }
#endif
}
