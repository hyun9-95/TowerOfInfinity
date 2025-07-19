#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class LobbyFlow : BaseFlow<LobbyFlowModel>
{
    public override UIType ViewType => UIType.LobbyView;
    public override FlowType FlowType => FlowType.LobbyFlow;


    private List<CharacterUnit> loadedCharacters = new List<CharacterUnit>();

    private LobbySceneManager lobbySceneManager = null;

    public override async UniTask LoadingProcess()
    {
        var loadedScene = await AddressableManager.Instance.LoadSceneAsyncWithName(Model.LobbySceneDefine, UnityEngine.SceneManagement.LoadSceneMode.Single);

        if (!loadedScene.IsValid())
            return;

        await UniTask.NextFrame();

        lobbySceneManager = loadedScene.GetRootComponent<LobbySceneManager>();

        if (lobbySceneManager == null)
        {
            Logger.Null(lobbySceneManager.name);
            return;
        }

        await LoadPlayerCharacters();
    }

    public override async UniTask Process()
    {
        InitializeBattlePortal();
        await ActiveCharacters();
        await ShowLobbyView();
    }

    public override async UniTask Exit()
    {
        await AddressableManager.Instance.UnloadSceneAsync(Model.LobbySceneDefine);
    }


    private async UniTask LoadPlayerCharacters()
    {
        var playerTransform = lobbySceneManager.PlayerStartTransform;
        var leaderCharacter = await PlayerManager.Instance.CreateLeaderCharacter(playerTransform);

        await UniTask.NextFrame();

        lobbySceneManager.SetFollowCamera(leaderCharacter.transform);

        if (leaderCharacter != null)
        {
            leaderCharacter.Initialize();
            loadedCharacters.Add(leaderCharacter);
        }
    }

    private void InitializeBattlePortal()
    {
        var battlePortal = lobbySceneManager.GetBattlePortal();

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
        LobbyController lobbyController = new LobbyController();
        LobbyViewModel lobbyViewModel = new LobbyViewModel();
        lobbyController.SetModel(lobbyViewModel);

        await UIManager.Instance.ChangeView(lobbyController, true);
    }

    private void WarpToBattle(Collider2D collider2D)
    {
        BattleFlowModel battleFlowModel = new BattleFlowModel();
        battleFlowModel.SetBattleSceneDefine(SceneDefine.Battle_Atlantis);
        battleFlowModel.SetDataDungeon(DataManager.Instance.GetDataById<DataDungeon>((int)DungeonDefine.DUNGEON_ATLANTIS));

        FlowManager.Instance.ChangeFlow(FlowType.BattleFlow, battleFlowModel).Forget();
    }
}
