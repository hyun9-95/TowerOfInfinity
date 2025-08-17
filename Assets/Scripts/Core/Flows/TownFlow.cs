#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class TownFlow : BaseFlow<TownFlowModel>
{
    public override UIType ViewType => UIType.TownView;
    public override FlowType FlowType => FlowType.TownFlow;


    private List<CharacterUnit> loadedCharacters = new List<CharacterUnit>();

    private TownSceneManager townSceneManager = null;

    public override async UniTask LoadingProcess()
    {
        if (!loadedScene.IsValid())
            return;

        await UniTask.NextFrame();

        townSceneManager = loadedScene.GetRootComponent<TownSceneManager>();

        if (townSceneManager == null)
        {
            Logger.Null(townSceneManager.name);
            return;
        }

        await LoadPlayerCharacters();
    }

    public override async UniTask Process()
    {
        InitializeBattlePortal();
        await ActiveCharacters();
        await ShowLobbyView();

        InputManager.EnableMoveInput(true);
    }

    public override async UniTask Exit()
    {
        InputManager.EnableMoveInput(false);
    }

    private async UniTask LoadPlayerCharacters()
    {
        await PlayerManager.Instance.UpdateMainPlayerCharacter(CharacterSetUpType.Town);

        var playerTransform = townSceneManager.PlayerStartTransform;
        var mainCharacter = PlayerManager.Instance.GetMainPlayerCharacterUnit();
        mainCharacter.transform.SetPositionAndRotation(playerTransform.position, Quaternion.identity);

        await UniTask.NextFrame();

        townSceneManager.SetFollowCamera(mainCharacter.transform);

        if (mainCharacter != null)
        {
            mainCharacter.Initialize();
            loadedCharacters.Add(mainCharacter);
        }

        TownSceneManager.Instance.SetPlayerCharacter(mainCharacter);
    }

    private void InitializeBattlePortal()
    {
        var battlePortal = townSceneManager.GetBattlePortal();

        if (battlePortal != null)
            battlePortal.Model.SetOnTriggerEnter(WarpToBattle);
    }

    public async UniTask ActiveCharacters()
    {
        foreach (var loadedCharacter in loadedCharacters)
            loadedCharacter.Activate();
    }

    public async UniTask ShowLobbyView()
    {
        TownViewController lobbyController = new TownViewController();
        TownViewModel lobbyViewModel = new TownViewModel();
        lobbyController.SetModel(lobbyViewModel);

        await UIManager.Instance.ChangeView(lobbyController, true);
    }

    private void WarpToBattle(Collider2D collider2D)
    {
        InputManager.EnableMoveInput(false);

        BattleFlowModel battleFlowModel = new BattleFlowModel();
        battleFlowModel.SetSceneDefine(SceneDefine.Battle_Atlantis);
        battleFlowModel.SetDataDungeon(DataManager.Instance.GetDataById<DataDungeon>((int)DungeonDefine.DUNGEON_ATLANTIS));

        FlowManager.Instance.ChangeFlow(FlowType.BattleFlow, battleFlowModel).Forget();
    }
}
