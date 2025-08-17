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
    }

    public override async UniTask Process()
    {
        await LoadResources();
        await LoadDatas();
        await LoadUser();

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
        await AbilityBalanceFactory.Instance.Initialize();
        await BattleEventBalanceFactory.Instance.Initialize();
    }

    private async UniTask LoadUser()
    {
        Model.SetLoadingState(IntroViewModel.LoadingState.UserLoading);
        View.UpdateLoadingUI();

        // 유저 로드
        PlayerManager.Instance.LoadUser();
        PlayerManager.Instance.LoadUserSettings();

        // 유저 로컬 세팅
        InitializeLocalization();

        // 메인 캐릭터 로드
        await PlayerManager.Instance.LoadMainPlayerCharacter();
    }

    private void InitializeLocalization()
    {
        var applyLocal = PlayerManager.Instance.UserSettings.GetLocalizationType();

#if CHEAT
        if (CheatManager.CheatConfig.testLocalType != LocalizationType.None)
            applyLocal = CheatManager.CheatConfig.testLocalType;
#endif

        if (applyLocal == LocalizationType.None)
            applyLocal = LocalizationType.English;

        LocalizationManager.Instance.Initialize(applyLocal);
    }
}
