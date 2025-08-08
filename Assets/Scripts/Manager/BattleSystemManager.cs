#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using Unity.Android.Gradle.Manifest;
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
        await InitilalizeExpGainer(battleTeam.CurrentCharacter);

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

    private async UniTask InitilalizeExpGainer(CharacterUnit characterUnit)
    {
        expGainer = characterUnit.GetComponent<BattleExpGainer>();

        if (expGainer == null)
        {
            expGainer = await AddressableManager.Instance.InstantiateAddressableMonoAsync<BattleExpGainer>(
                PathDefine.CHARACTER_EXP_GAINER, characterUnit.transform);

            expGainer.SetModel(new BattleExpGainerModel());
        }

        expGainer.Model.SetOnExpGain(OnExpGain);
        expGainer.Model.SetOwner(characterUnit.Model);
        expGainer.Activate(true);
    }

    private async UniTask InitializeDamageGroup()
    {
        if (damageNumbersGroup == null)
        {
            damageNumbersGroup = await AddressableManager.Instance.
                InstantiateAddressableMonoAsync<DamageNumbersGroup>(PathDefine.DAMAGE_GROUP, objectContainer);
        }

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
        expGainer.UpdateRadius();

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
    }

    public void OnDamage(CharacterUnitModel sender, CharacterUnitModel receiver, float value, DamageType damageType)
    {
        if (receiver == null || receiver.IsDead)
            return;

        var damageAmount = Formula.GetDamageAmount(sender, receiver, value, damageType);

        var finalDamage = damageAmount;
        receiver.AddDamage(finalDamage);

        Logger.BattleLog($"{sender.CharacterDefine} -> {receiver.CharacterDefine} : {finalDamage} / Remain HP : {receiver.Hp}");

        // 적인 경우에만 대미지를 표시.
        if (receiver.TeamTag == TeamTag.Enemy)
            damageNumbersGroup.ShowDamage(damageType, receiver.Transform, finalDamage.ToString());

        OnHitEffect(receiver, damageType);
    }

    public void OnHitEffect(CharacterUnitModel receiver, DamageType damageType)
    {
        if (!receiver.IsDead)
            receiver.ActionHandler.OnHitEffectAsync(GetHitColorByDamageType(damageType)).Forget();
    }

    private Color GetHitColorByDamageType(DamageType damageType)
    {
        return damageType switch
        {
            DamageType.Heal => new Color32(100, 255, 100, 255),
            DamageType.Poison => new Color32(255, 100, 255, 255),
            _ => new Color32(255, 100, 100, 255),
        };
    }
}