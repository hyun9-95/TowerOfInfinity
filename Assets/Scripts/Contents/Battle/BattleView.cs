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
    }

    private void ShowSlider()
    {
        if (Model.BattleExp == 0)
        {
            expSlider.value = 0;
            return;
        }

        expSlider.value = Model.BattleExp / Model.NextBattleExp;
    }

    private void FixedUpdate()
    {
        if (!isShowing)
            return;

        if (Model == null)
            return;

        if (!BattleSystemManager.Instance.InBattle)
            return;

        timeText.SafeSetText(Model.GetElapsedTimeText());
    }
}
