#pragma warning disable CS1998

using Cysharp.Threading.Tasks;
using UnityEngine;

public class IntroController : BaseController<IntroViewModel>
{
    public override UIType UIType => UIType.IntroView;

    public override UICanvasType UICanvasType => UICanvasType.View;

    public bool IsSuccess => Model.DataLoader.CurrentState == BaseDataLoader.State.Success;

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
    }

    private void InitializeDataLoader()
    {
        switch (Model.LoadDataType)
        {
            case LoadDataType.LocalPath:
                LocalDataLoader localDataLoader = new();
                localDataLoader.SetLocalJsonDataPath(PathDefine.Json);
                localDataLoader.SetOnSuccessLoadData(OnSuccessDataLoader);
                Model.SetLocalDataLoader(localDataLoader);
                break;
        }
    }

    private void OnSuccessDataLoader()
    {
    }

    private async UniTask LoadResources()
    {
        Model.SetLoadingState(IntroViewModel.LoadingState.ResourceLoading);
        View.UpdateLoadingUI();

        if (Model.LoadDataType == LoadDataType.LocalPath)
        {
            AddressableManager.Instance.LoadLocalAddressableBuildAsync();
        }
    }

    private async UniTask LoadDatas()
    {
        Model.DataLoader.LoadData().Forget();
        View.ShowDataLoadingProgress().Forget();

        await UniTask.WaitUntil(() => { return !Model.DataLoader.IsLoading; });

        DataManager.Instance.GenerateDataContainerByDataDic(Model.LocalDataLoader.DicJsonByFileName);
    }

    private async UniTask LoadUser()
    {
        Model.SetLoadingState(IntroViewModel.LoadingState.UserLoading);
        View.UpdateLoadingUI();

        PlayerManager.Instance.LoadUser();
    }
}
