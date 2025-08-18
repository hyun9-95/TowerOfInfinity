#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class BattleFlow : BaseFlow<BattleFlowModel>
{
    public override UIType ViewType => UIType.TownView;
    public override FlowType FlowType => FlowType.BattleFlow;

    private BattleSceneManager battleSceneManager;
    private BattleSystemManager battleSystemManager;
    private BattleUIManager battleUIManager;
    private float attackCoolTime = 0;
    private float rollCoolTime = 0;

    private BattleInfo battleInfo;
    private BattleTeam battleTeam;

    public override async UniTask LoadingProcess()
    {
        battleSceneManager = loadedScene.GetRootComponent<BattleSceneManager>();
        
        if (battleSceneManager == null)
        {
            Logger.Null($"BattleBackgroundSceneManager");
            return;
        }

        // BattleSceneManager 하위의 매니저들
        battleSystemManager = BattleSystemManager.Instance;
        battleUIManager = BattleUIManager.Instance;

        battleTeam = await CreatePlayerBattleTeam(PlayerManager.Instance.User.UserCharacterInfo.CurrentDeck,
            battleSceneManager.PlayerStartTransform);

        battleInfo = CreateBattleInfo(Model.DataDungeon, battleTeam);
       
        await battleSystemManager.Prepare(battleInfo);
        await battleSceneManager.Prepare(battleInfo);
        await battleUIManager.Prepare();
    }

    public override async UniTask Process()
    {
        await battleSystemManager.StartBattle();
        await battleSceneManager.StartSpawn();
        await battleUIManager.ShowHpBar(battleInfo.MainCharModel);
        EnableBattleInput();
    }

    public override async UniTask Exit()
    {
        DisableBattleInput();
        battleSceneManager.StopSpawn();
    }

    private void EnableBattleInput()
    {
        var mainCharacter = battleInfo.MainCharacter;
        attackCoolTime = mainCharacter.GetPrimaryWeaponCoolTime();
        rollCoolTime = FloatDefine.DEAFAULT_ROLL_COOTIME;

        InputManager.SetActionCoolTime(ActionInput.Attack, attackCoolTime);
        InputManager.SetActionCoolTime(ActionInput.Roll, rollCoolTime);
        InputManager.EnableMoveInput(true);
        InputManager.EnableActionButtons(true);
    }

    private void DisableBattleInput()
    {
        InputManager.SetActionCoolTime(ActionInput.Attack, 0);
        InputManager.SetActionCoolTime(ActionInput.Roll, 0);
        InputManager.EnableMoveInput(false);
        InputManager.EnableActionButtons(false);
    }

    private BattleInfo CreateBattleInfo(DataDungeon dataDungeon, BattleTeam battleTeam)
    {
        var battleInfo = new BattleInfo();
        battleInfo.SetBattleTeam(battleTeam);
        battleInfo.SetDataDungeon(dataDungeon);
        battleInfo.SetExpTable();
        battleInfo.SetLevel(0);
        battleInfo.SetBattleExp(0);

        return battleInfo;
    }

    private async UniTask<BattleTeam> CreatePlayerBattleTeam(SubCharacterInfo[] currentDeck, Transform playerTransform)
    {
        await PlayerManager.Instance.UpdateMainPlayerCharacter(CharacterSetUpType.Battle);

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
}
