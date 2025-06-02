#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BattleProcessManager : BaseMonoManager<BattleProcessManager>
{
    #region Property
    public BattleInfo BattleInfo { get; private set; } = new BattleInfo();
    #endregion

    #region Value
    private BattleExpGainer expGainer;
    private BattleViewController battleUIController;
    private BattleViewModel battleViewModel;
    #endregion

    public async UniTask StartBattle(CharacterUnit characterUnit)
    {
        expGainer = characterUnit.GetComponentInChildren<BattleExpGainer>();

        if (expGainer == null)
            return;

        BattleInfo BattleInfo = new BattleInfo();
        BattleInfo.SetExpTable();
        BattleInfo.SetLevel(0);
        BattleInfo.SetBattleExp(0);

        expGainer.Model.SetOnExpGain(OnExpGain);
        expGainer.Model.SetOwner(characterUnit.Model);
        expGainer.Activate(true);

        if (UIManager.Instance.GetCurrentController() is BattleViewController controller)
        {
            battleUIController = controller;
            battleViewModel = controller.GetModel<BattleViewModel>();
        }
    }

    private void OnExpGain(float exp)
    {
        var expGainerModel = expGainer.Model;

        expGainerModel.SetOnExpGain(BattleInfo.OnExpGain);

        if (expGainerModel.Level != BattleInfo.Level)
        {
            expGainerModel.SetLevel(BattleInfo.Level);
            expGainer.UpdateRadius();
            OnChangeLevel();
        }
    }

    private void OnChangeLevel()
    {
        var battleUIModel = battleUIController.GetModel<BattleViewModel>();
        battleUIModel.SetLevel(BattleInfo.Level);
        battleUIModel.SetBattleExp(BattleInfo.BattleExp);
        battleUIModel.SetNextBattleExp(BattleInfo.NextBattleExp);

        battleUIController.Refresh().Forget();
    }

    private void FixedUpdate()
    {
        
    }

    private void UpdateProcess()
    {

    }
}