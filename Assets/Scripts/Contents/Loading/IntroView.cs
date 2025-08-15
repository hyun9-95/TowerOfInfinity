using Cysharp.Threading.Tasks;
using UnityEngine;

public class IntroView : BaseView
{
    private IntroViewModel Model => GetModel<IntroViewModel>();

    [SerializeField]
    private AddressableLoader loadingBarLoader;

    [SerializeField]
    private GameObject touchToStart;

    private LoadingBar loadingBar;

    public async UniTask ShowDataLoadingProgress()
    {
        await LoadLoadingBar();

        while (Model.DataLoader.IsLoading)
        {
            UpdateLoadingUI();
            await UniTask.NextFrame(TokenPool.Get(GetHashCode()));
        }
    }

    private async UniTask LoadLoadingBar()
    {
        loadingBar = await loadingBarLoader.InstantiateAsyc<LoadingBar>();
        loadingBar.SetComplete(false);
    }

    public void UpdateLoadingUI()
    {
        if (loadingBar == null)
            return;

        loadingBar.SetLoadingProgressText(Model.GetLoadingProgressText());
        loadingBar.SetLoadingProgress(Model.DataLoader.CurrentProgressValue);
    }

    public void ShowComplete(bool value)
    {
        loadingBar.SetComplete(value);
        touchToStart.SafeSetActive(value);
    }

    public void OnClickStart()
    {
        Model.OnCompleteLoading?.Invoke();
    }
}
