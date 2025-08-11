#pragma warning disable CS1998  
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleResultView : BaseView
{
    public BattleResultViewModel Model => GetModel<BattleResultViewModel>();

    [SerializeField]
    private TextMeshProUGUI resultText;

    [SerializeField]
    private TextMeshProUGUI killCountText;

    [SerializeField]
    private TextMeshProUGUI timeText;

    public override async UniTask ShowAsync()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        killCountText.SafeSetText(Model.GetKillCountText());
        timeText.SafeSetText(Model.GetElapsedTimeText());
    }
}