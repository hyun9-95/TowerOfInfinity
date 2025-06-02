using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IntroView : BaseView
{
    private IntroViewModel Model => GetModel<IntroViewModel>();

    [SerializeField]
    private TextMeshProUGUI progressText;

    [SerializeField]
    private Slider progressBar;

    public async UniTask ShowDataLoadingProgress()
    {
        while (Model.DataLoader.IsLoading)
        {
            UpdateLoadingUI();
            await UniTask.NextFrame(TokenPool.Get(GetHashCode()));
        }
    }

    public void UpdateLoadingUI()
    {
        progressText.SafeSetText(Model.GetLoadingProgressText());
        progressBar.value = Model.DataLoader.CurrentProgressValue;
    }
}
