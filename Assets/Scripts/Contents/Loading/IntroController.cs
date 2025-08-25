#pragma warning disable CS1998

using Cysharp.Threading.Tasks;
using UnityEngine;

public class IntroController : BaseController<IntroViewModel>
{
    public override UIType UIType => UIType.IntroView;

    public override UICanvasType UICanvasType => UICanvasType.View;

    private IntroView View => GetView<IntroView>();

    public override void Enter()
    {
        InitializeDataLoader();
        Model.SetOnClickNewGame(OnClickNewGame);
        Model.SetOnClickContinue(OnClickContinue);
        Model.SetOnClickSettings(OnClickSettings);
        Model.SetOnClickExit(OnClickExit);
    }

    public override async UniTask Process()
    {
        await LoadResources();
        await LoadDatas();
        LoadUser();

        View.ShowComplete(true);
    }

    private void InitializeDataLoader()
    {
        switch (Model.LoadDataType)
        {
#if UNITY_EDITOR
            case LoadDataType.Editor:
                EditorDataLoader editorData = new();
                editorData.SetLocalJsonDataPath(PathDefine.Json);
                editorData.SetOnSuccessLoadData(OnSuccessDataLoader);
                Model.SetEditorDataLoader(editorData);
                break;
#endif

            case LoadDataType.Addressable:
                AddressableDataLoader addressableDataLoader = new();
                addressableDataLoader.SetOnSuccessLoadData(OnSuccessDataLoader);

                // 뷰를 트래커로 할당해서, View가 파괴될 때 자동으로 TextAsset들을 해제한다.
                addressableDataLoader.SetAddressableTracker(View);
                Model.SetAddressableDataLoader(addressableDataLoader);
                break;
        }
    }

    private void OnSuccessDataLoader()
    {
    }

    private async UniTask LoadResources()
    {
        Model.SetLoadingState(IntroViewModel.LoadingState.ResourceLoading);
        
        // 추후 어드레서블 리모트 사용 시 이 부분에 다운로드 / 무결성 체크 구현

        View.UpdateLoadingUI();
    }

    private async UniTask LoadDatas()
    {
        Model.DataLoader.LoadData().Forget();
        View.ShowDataLoadingProgress().Forget();

        await UniTask.WaitUntil(() => { return !Model.DataLoader.IsLoading; });

        DataManager.Instance.GenerateDataContainerByDataDic(Model.DataLoader.DicJsonByFileName);
        LocalizationManager.Instance.Initialize();

        await AbilityBalanceFactory.Instance.Initialize();
        await BattleEventBalanceFactory.Instance.Initialize();
    }

    private void LoadUser()
    {
        Model.SetLoadingState(IntroViewModel.LoadingState.UserLoading);
        View.UpdateLoadingUI();

        // 유저 로드
        PlayerManager.Instance.LoadUser();

        Model.SetShowContinue(PlayerManager.Instance.User.IsCompletePrologue);
    }

    private async UniTask StartGameAsync()
    {
        Model.OnEnterGame();
    }

    private void OnClickNewGame()
    {
        PlayerManager.Instance.LoadUser(true);
        StartGameAsync().Forget();
    }

    private void OnClickContinue()
    {
        StartGameAsync().Forget();
    }

    private void OnClickSettings()
    {
        UserSettingPopupModel model = new UserSettingPopupModel();
        model.SetMasterVolume(UserSettings.Volume.Master);
        model.SetVolume(SoundType.Bgm, UserSettings.Volume.Bgm);
        model.SetVolume(SoundType.Amb, UserSettings.Volume.Amb);
        model.SetVolume(SoundType.Sfx, UserSettings.Volume.Sfx);
        model.SetLocalizationType(UserSettings.Localization.Type);

        UserSettingPopupController controller = new UserSettingPopupController();
        controller.SetModel(model);

        UIManager.Instance.OpenPopup(controller).Forget();
    }   

    private void OnClickExit()
    {
        Application.Quit();
    }
}
