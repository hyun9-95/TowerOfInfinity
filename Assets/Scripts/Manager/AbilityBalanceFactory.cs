using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

public class AbilityBalanceFactory : BaseManager<AbilityBalanceFactory>
{
    private Dictionary<AbilityDefine, ScriptableAbilityBalance> abilityBalanceDic = new Dictionary<AbilityDefine, ScriptableAbilityBalance>();

    public async UniTask Initialize()
    {
        foreach (AbilityDefine define in Enum.GetValues(typeof(AbilityDefine)))
        {
            if (define == AbilityDefine.None)
                continue;

            var abilityBalance = await AddressableManager.Instance.LoadScriptableObject<ScriptableAbilityBalance>(define.ToString(), false);
            
            if (abilityBalance != null)
            {
                abilityBalanceDic[define] = abilityBalance;
            }
            else
            {
                Logger.Error("Failed to load ability balance data for: " + define);
            }
        }
    }

    public ScriptableAbilityBalance GetAbilityBalance(int id)
    {
        AbilityDefine define = (AbilityDefine)id;
        
        if (abilityBalanceDic.TryGetValue(define, out var abilityBalance))
            return abilityBalance;

        return null;
    }

    public void Clear()
    {
        abilityBalanceDic.Clear();
    }
}