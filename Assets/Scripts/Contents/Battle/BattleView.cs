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

    public override async UniTask ShowAsync()
    {
        UpdateUI();
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
}
