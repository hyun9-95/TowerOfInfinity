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
        var loadedScene = await AddressableManager.Instance.LoadSceneAsync(Model.LobbySceneDefine, UnityEngine.SceneManagement.LoadSceneMode.Single);

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
    }

    public override async UniTask Exit()
    {
        await AddressableManager.Instance.UnloadSceneAsync(Model.LobbySceneDefine);
    }


    private async UniTask LoadPlayerCharacters()
    {
        var playerTransform = townSceneManager.PlayerStartTransform;
        var mainCharacter = await PlayerManager.Instance.GetMainCharacter();
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
        BattleFlowModel battleFlowModel = new BattleFlowModel();
        battleFlowModel.SetBattleSceneDefine(SceneDefine.Battle_Atlantis);
        battleFlowModel.SetDataDungeon(DataManager.Instance.GetDataById<DataDungeon>((int)DungeonDefine.DUNGEON_ATLANTIS));

        FlowManager.Instance.ChangeFlow(FlowType.BattleFlow, battleFlowModel).Forget();
    }
}
