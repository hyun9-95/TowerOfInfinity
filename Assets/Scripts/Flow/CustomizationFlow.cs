#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class CustomizationFlow : BaseFlow<CustomizationFlowModel>
{
    public override UIType ViewType => UIType.CharacterCustomizationView;
    public override FlowType FlowType => FlowType.CustomizationFlow;

    private List<CharacterUnit> loadedCharacters = new List<CharacterUnit>();
    private TownSceneManager townSceneManager = null;

    public override async UniTask LoadingProcess()
    {
        // 1. Town_Customization 어드레서블 씬 로드
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

    private async UniTask ActiveCharacters()
    {
        foreach (var loadedCharacter in loadedCharacters)
            loadedCharacter.Activate();
    }

    private async UniTask ShowCustomizationView()
    {
        // 4. 현재 저장된 유저의 캐릭터 정보 (없다면 Human으로) 설정
        var userCharacterInfo = GetUserCharacterInfo();
        Model.SetUserCharacterInfo(userCharacterInfo);

        // 5. UIManager.Instance.ChangeView로 CharacterCustomizationView 보여줌
        CharacterCustomizationController customizationController = new CharacterCustomizationController();
        CharacterCustomizationViewModel customizationViewModel = new CharacterCustomizationViewModel();
        
        // SpriteLibrary와 UserCharacterInfo를 ViewModel에 전달
        customizationViewModel.SetSpriteLibrary(townSceneManager.PlayerCharacter.SpriteLibrary);
        customizationViewModel.SetUserCharacterInfo(Model.UserCharacterInfo);
        
        customizationController.SetModel(customizationViewModel);

        await UIManager.Instance.ChangeView(customizationController, true);
    }

    private UserCharacterAppearanceInfo GetUserCharacterInfo()
    {
        // 저장된 사용자 캐릭터 정보가 있다면 로드, 없다면 Human으로 기본 설정
        // TODO: PlayerManager나 저장소에서 실제 데이터 로드
        var userCharacterInfo = new UserCharacterAppearanceInfo();
        
        // 기본값을 "Human"으로 설정 (모든 파츠를 Human으로 초기화)
        for (int i = 0; i < userCharacterInfo.parts.Length; i++)
        {
            userCharacterInfo.parts[i] = "Human";
        }

        return userCharacterInfo;
    }
}