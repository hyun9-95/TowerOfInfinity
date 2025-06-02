using UnityEngine;
using DamageNumbersPro;

public class DamageNumbersGroup : AddressableMono
{
    [SerializeField]               
    private DamageNumberMesh[] damageNumbers;

    public void PrewarmPool()
    {
        for (DamageType type = DamageType.Normal; type < DamageType.Max; type++)
        {
            if (damageNumbers[(int)type] != null)
                damageNumbers[(int)type].PrewarmPool();
        }
    }

    public void ShowDamage(DamageType damageType, Transform targetTr, string text)
    {
        var prefab = damageNumbers[(int)damageType];

        if (prefab == null)
            return;

        prefab.Spawn(targetTr.position, text, targetTr);
    }
}
