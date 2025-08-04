#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class CustomizationFlow : BaseFlow<CustomizationFlowModel>
{
    public override UIType ViewType => UIType.CharacterCustomizationView;
    public override FlowType FlowType => FlowType.CustomizationFlow;

    private List<CharacterUnit> loadedCharacters = new List<CharacterUnit>();
    private TownSceneManager townSceneManager = null;

    public override async UniTask LoadingProcess()
    {
        var loadedScene = await AddressableManager.Instance.LoadSceneAsync(SceneDefine.Town_Customization, UnityEngine.SceneManagement.LoadSceneMode.Single);

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
        await ActiveCharacters();
        await ShowCustomizationView();
    }

    public override async UniTask Exit()
    {
        await AddressableManager.Instance.UnloadSceneAsync(SceneDefine.Town_Customization);
    }

    private async UniTask LoadPlayerCharacters()
    {
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

    private async UniTask ActiveCharacters()
    {
        foreach (var loadedCharacter in loadedCharacters)
            loadedCharacter.Activate();
    }

    private async UniTask ShowCustomizationView()
    {
        CharacterCustomizationController customizationController = new CharacterCustomizationController();
        CharacterCustomizationViewModel customizationViewModel = new CharacterCustomizationViewModel();
        customizationController.SetModel(customizationViewModel);

        await UIManager.Instance.ChangeView(customizationController, true);
    }
}