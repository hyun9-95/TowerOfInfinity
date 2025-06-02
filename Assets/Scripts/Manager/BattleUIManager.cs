#pragma warning disable CS1998

using Cysharp.Threading.Tasks;
using UnityEngine;

public class BattleUIManager : BaseMonoManager<BattleUIManager>
{
    [SerializeField]
    private Transform damageParent;

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
}