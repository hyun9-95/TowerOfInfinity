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
        if (killCountText != null)
            killCountText.text = $"Kills: {Model.KillCount}";

        if (timeText != null)
        {
            int minutes = Mathf.FloorToInt(Model.ElapsedTime / 60f);
            int seconds = Mathf.FloorToInt(Model.ElapsedTime % 60f);
            timeText.text = $"Time: {minutes:00}:{seconds:00}";
        }
    }
}