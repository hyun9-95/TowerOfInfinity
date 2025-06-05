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
            battleViewModel.SetOnChangeCharacter(OnChangeCharacter);
        }
    }

    private void OnExpGain(float exp)
    {
        var expGainerModel = expGainer.Model;

        BattleInfo.OnExpGain(exp);

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

    private void OnChangeCharacter(CharacterUnit changeTarget)
    {
        if (changeTarget == null)
            return;

        CharacterUnit origin = BattleInfo.CurrentCharacter;

        var originModel = origin.Model;
        var changeTargetModel = changeTarget.Model;

        // 무기의 Owner 변경
        foreach (var weapon in originModel.Weapons)
        {
            weapon.Model.SetOwner(changeTargetModel);
            changeTargetModel.AddWeapon(weapon);
        }

        // expGainer의 Owner 변경
        expGainer.Model.SetOwner(changeTargetModel);
    }

    private void FixedUpdate()
    {
        
    }

    private void UpdateProcess()
    {

    }
}