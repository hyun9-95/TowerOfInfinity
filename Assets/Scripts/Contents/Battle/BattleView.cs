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

    public override async UniTask ShowAsync()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        ShowSlider();
        ShowTimer();
        ShowKillCount();
    }

    private void ShowSlider()
    {
        if (expSlider == null)
            return;

        expSlider.value = Model.BattleExp / Model.NextBattleExp;
    }

    private void ShowTimer()
    {
        if (timeText == null)
            return;

        int minutes = Mathf.FloorToInt(Model.ElapsedTime / 60f);
        int seconds = Mathf.FloorToInt(Model.ElapsedTime % 60f);
        timeText.text = $"{minutes:00}:{seconds:00}";
    }

    private void ShowKillCount()
    {
        if (killCountText == null)
            return;

        killCountText.text = $"Kill: {Model.KillCount}";
    }
}
