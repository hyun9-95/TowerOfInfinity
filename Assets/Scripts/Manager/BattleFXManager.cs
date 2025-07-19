#pragma warning disable CS1998

using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 대미지나 효과등의 단발성 UI나 이펙트를 관리한다.
/// </summary>
public class BattleFXManager : BaseMonoManager<BattleFXManager>
{
    [SerializeField]
    private Transform damageParent;

    [SerializeField]
    private HpBarUnit hpBarUnit;

    private DamageNumbersGroup damageNumbersGroup;

    public async UniTask Prepare()
    {
        await PrepareDamageGroup();
    }

    #region DamageNumbers
    private async UniTask PrepareDamageGroup()
    {
        if (damageNumbersGroup == null)
            damageNumbersGroup = await AddressableManager.Instance.InstantiateAddressableMonoAsync<DamageNumbersGroup>(typeof(DamageNumbersGroup).ToString(), damageParent);

        damageNumbersGroup.PrewarmPool();
    }

    public void ShowDamage(DamageType damageType, Transform tr, string text)
    {
        damageNumbersGroup.ShowDamage(damageType, tr, text);
    }
    #endregion

    #region HPBar
    public async UniTask ShowHpBar(CharacterUnitModel owner)
    {
        if (hpBarUnit.Model == null)
            hpBarUnit.SetModel(new HpBarUnitModel());

        hpBarUnit.Hide();
        
        var model = hpBarUnit.Model;
        model.SetOwner(owner);

        await hpBarUnit.ShowAsync();
    }
    #endregion
}