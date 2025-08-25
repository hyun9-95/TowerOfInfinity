using Cysharp.Threading.Tasks;
using UnityEngine;

public class IntroView : BaseView
{
    private IntroViewModel Model => GetModel<IntroViewModel>();

    [SerializeField]
    private AddressableLoader loadingBarLoader;

    [SerializeField]
    private GameObject menuPanel;

    [SerializeField]
    private GameObject buttonContinue;

    private LoadingBar loadingBar;

    public async UniTask ShowDataLoadingProgress()
    {
        await LoadLoadingBar();

        while (Model.DataLoader.IsLoading)
        {
            UpdateLoadingUI();
            await UniTaskUtils.NextFrame(TokenPool.Get(GetHashCode()));
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

        if (value)
            buttonContinue.SafeSetActive(Model.ShowContinue);

        menuPanel.SafeSetActive(value);
    }

    public void OnClickNewGame()
    {
        Model.OnClickNewGame();
    }

    public void OnClickContinue()
    {
        Model.OnClickContinue();
    }

    public void OnClickSettings()
    {
        Model.OnClickSettings();
    }

    public void OnClickExit()
    {
        Model.OnClickExit();
    }
}
