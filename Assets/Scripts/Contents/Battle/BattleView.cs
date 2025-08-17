#pragma warning disable CS1998  
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleView : BaseView
{
    public BattleViewModel Model => GetModel<BattleViewModel>();

    [SerializeField]
    private Slider expSlider;

    [SerializeField]
    private TextMeshProUGUI timeText;

    [SerializeField]
    private TextMeshProUGUI killCountText;

    [SerializeField]
    private TextMeshProUGUI waveText;

    private bool isShowing = false;

    public override async UniTask ShowAsync()
    {
        UpdateUI();
        isShowing = true;
    }

    public void UpdateUI()
    {
        ShowSlider();
        killCountText.SafeSetText(Model.GetKillCountText());
        waveText.SafeSetText(Model.GetWaveText());
        timeText.SafeSetText(Model.GetElapsedTimeText());
    }

    private void ShowSlider()
    {
        expSlider.value = Model.GetExpSliderValue();
    }

    private void LateUpdate()
    {
        if (!isShowing)
            return;

        if (Model == null)
            return;

        if (!BattleSystemManager.InBattle)
            return;

        UpdateUI();
    }
}
