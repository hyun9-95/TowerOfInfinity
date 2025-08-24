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
    private AbilitySlotUnit abilitySlotUnit;

    [SerializeField]
    private TextMeshProUGUI timeText;

    [SerializeField]
    private TextMeshProUGUI killCountText;

    [SerializeField]
    private TextMeshProUGUI waveText;

    [SerializeField]
    private TextMeshProUGUI bossNameText;

    [SerializeField]
    private TextMeshProUGUI hpText;

    [SerializeField]
    private Slider hpSlider;

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

        if (Model.BossModel != null)
            UpdateBossUI();
    }

    public void UpdateBossUI()
    {
        hpText.SafeSetText(Model.GetBossHpText());
        hpSlider.value = Model.GetBossHpSliderValue();
    }

    public void ShowBossUI(bool value)
    {
        if (value)
            bossNameText.SafeSetText(Model.BossName);

        bossNameText.gameObject.SafeSetActive(value);
        hpText.gameObject.SafeSetActive(value);
        hpSlider.gameObject.SafeSetActive(value);
    }

    private void ShowSlider()
    {
        expSlider.value = Model.GetExpSliderValue();
    }

    public void ShowAblitySlotUnit()
    {
        abilitySlotUnit.SetModel(Model.AbilitySlotUnitModel);
        abilitySlotUnit.ShowAsync().Forget();
    }
}
