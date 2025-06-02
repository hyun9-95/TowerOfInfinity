#pragma warning disable CS1998  
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class BattleView : BaseView
{
    public BattleViewModel Model => GetModel<BattleViewModel>();

    [SerializeField]
    private Slider expSlider;

    public override async UniTask ShowAsync()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        ShowSlider();
    }

    private void ShowSlider()
    {
        if (expSlider == null)
            return;

        expSlider.value = Model.BattleExp / Model.NextBattleExp;
    }
}
