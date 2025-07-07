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
    #endregion

    public async UniTask StartBattle(CharacterUnit characterUnit, BattleViewController viewController)
    {
        InitializBattleInfo();
        InitilalizeExpGainer(characterUnit);
        InitializeBattleViewEvent(viewController);
    }

    private void InitializBattleInfo()
    {
        BattleInfo = new BattleInfo();
        BattleInfo.SetExpTable();
        BattleInfo.SetLevel(0);
        BattleInfo.SetBattleExp(0);
    }

    private void InitilalizeExpGainer(CharacterUnit characterUnit)
    {
        expGainer = characterUnit.GetComponentInChildren<BattleExpGainer>();

        if (expGainer == null)
            return;

        expGainer.Model.SetOnExpGain(OnExpGain);
        expGainer.Model.SetOwner(characterUnit.Model);
        expGainer.Activate(true);
    }

    // 입력이 필요한 이벤트를 BattleViewController에 등록한다.
    private void InitializeBattleViewEvent(BattleViewController viewController)
    {
        var viewModel = viewController.GetModel<BattleViewModel>();
        viewModel.SetOnChangeCharacter(OnChangeCharacter);
    }

    private void OnExpGain(float exp)
    {
        var expGainerModel = expGainer.Model;

        BattleInfo.OnExpGain(exp);

        if (expGainerModel.Level != BattleInfo.Level)
        {
            expGainerModel.SetLevel(BattleInfo.Level);
            expGainer.UpdateRadius();
        }

        BattleObserverParam param = new BattleObserverParam();
        param.SetBattleInfo(BattleInfo);

        // 경험치 획득 시 UI 갱신해준다.
        ObserverManager.NotifyObserver(BattleObserverID.ExpGain, param);
    }

    private void OnChangeCharacter(CharacterUnitModel changeTargetModel)
    {
        if (changeTargetModel == null)
            return;

        CharacterUnit origin = BattleInfo.CurrentCharacter;

        var originModel = origin.Model;

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