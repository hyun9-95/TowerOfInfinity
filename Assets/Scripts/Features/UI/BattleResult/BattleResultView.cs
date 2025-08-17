#pragma warning disable CS1998  
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class BattleResultView : BaseView
{
    public BattleResultViewModel Model => GetModel<BattleResultViewModel>();

    [SerializeField]
    private GameObject victoryObject;

    [SerializeField]
    private GameObject defeatObject;

    [SerializeField]
    private TextMeshProUGUI timeText;

    [SerializeField]
    private TextMeshProUGUI killCountText;

    public override async UniTask ShowAsync()
    {
        ShowBattleResult();
    }

    private void ShowBattleResult()
    {
        switch (Model.BattleResult)
        {
            case BattleResult.Victory:
                ShowVictory();
                break;

            case BattleResult.None:
            case BattleResult.Defeat:
                ShowDefeat();
                break;
        }

        ShowEtcText();
    }

    private void ShowVictory()
    {
        victoryObject.SafeSetActive(true);
        defeatObject.SafeSetActive(false);
    }

    private void ShowDefeat()
    {
        defeatObject.SafeSetActive(true);
        victoryObject.SafeSetActive(false);
    }

    private void ShowEtcText()
    {
        killCountText.SafeSetText(Model.GetKillCountText());
        timeText.SafeSetText(Model.GetElapsedTimeText());
    }

    public void OnReturnToTown()
    {
        Model.OnReturnToTown();
    }
}