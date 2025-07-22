#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 전반적인 전투 로직을 관리한다.
/// </summary>
public class BattleSystemManager : BaseManager<BattleSystemManager>
{
    #region Property
    public BattleInfo BattleInfo { get; private set; } = new BattleInfo();
    #endregion

    #region Value
    private BattleExpGainer expGainer;
    private DamageNumbersGroup damageNumbersGroup;
    private Transform objectContainer;
    #endregion

    public async UniTask Prepare(BattleTeam battleTeam, Transform objectContainerValue)
    {
        InitializBattleInfo(battleTeam);
        InitilalizeExpGainer(battleTeam.CurrentCharacter);

        objectContainer = objectContainerValue;
        await InitializeDamageGroup();
    }

    private void InitializBattleInfo(BattleTeam battleTeam)
    {
        BattleInfo = new BattleInfo();
        BattleInfo.SetBattleTeam(battleTeam);
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

    private async UniTask InitializeDamageGroup()
    {
        if (damageNumbersGroup == null)
            damageNumbersGroup = await AddressableManager.Instance.InstantiateAddressableMonoAsync<DamageNumbersGroup>(typeof(DamageNumbersGroup).Name, objectContainer);

        damageNumbersGroup.PrewarmPool();
    }

    // 입력이 필요한 이벤트를 BattleViewModel에 등록한다.
    public void BindingBattleViewEvent(BattleViewModel viewModel)
    {
        if (viewModel == null)
            return;

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

    private void OnChangeCharacter(int changeIndex)
    {
        var changeTarget = BattleInfo.BattleTeam.GetCharacterUnit(changeIndex);

        if (changeTarget == null)
            return;

        var changeTargetModel = changeTarget.Model;

        CharacterUnit origin = BattleInfo.BattleTeam.CurrentCharacter;

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

    public void OnDamage(CharacterUnitModel sender, CharacterUnitModel receiver, float value, DamageType damageType)
    {
        var damageAmount = Formula.GetDamageAmount(sender, receiver, value, damageType);

        var finalDamage = damageAmount;
        receiver.AddDamage(finalDamage);

        Logger.BattleLog($"{sender.CharacterDefine} -> {receiver.CharacterDefine} : {finalDamage} / Remain HP : {receiver.Hp}");

        // 적인 경우에만 대미지를 표시.
        if (receiver.TeamTag == TeamTag.Enemy)
            damageNumbersGroup.ShowDamage(damageType, receiver.Transform, finalDamage.ToString());
    }
}