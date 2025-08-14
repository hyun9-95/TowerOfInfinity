using Cysharp.Threading.Tasks;
using System.Net;
using UnityEngine;

public class IntroView : BaseView
{
    private IntroViewModel Model => GetModel<IntroViewModel>();

    [SerializeField]
    private AddressableLoader loadingBarLoader;

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
    }

    public void UpdateLoadingUI()
    {
        if (loadingBar == null)
            return;

        loadingBar.SetLoadingProgressText(Model.GetLoadingProgressText());
        loadingBar.SetLoadingProgress(Model.DataLoader.CurrentProgressValue);
    }
}
