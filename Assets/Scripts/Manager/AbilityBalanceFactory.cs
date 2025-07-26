using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Text;

public class AbilityBalanceFactory : BaseManager<AbilityBalanceFactory>
{
    private Dictionary<AbilityDefine, ScriptableAbilityBalance> abilityBalanceDic = new Dictionary<AbilityDefine, ScriptableAbilityBalance>();
    private string folderPath = "AbilityCore/AbilityBalance/";

    public async UniTask Initialize()
    {
        StringBuilder sb = new StringBuilder();
        foreach (AbilityDefine define in Enum.GetValues(typeof(AbilityDefine)))
        {
            if (define == AbilityDefine.None)
                continue;

            string balancePath = sb.Append(folderPath).Append(define.ToString()).ToString();
            var abilityBalance = await AddressableManager.Instance.LoadScriptableObject<ScriptableAbilityBalance>(balancePath, false);
            
            if (abilityBalance != null)
            {
                abilityBalanceDic[define] = abilityBalance;
            }
            else
            {
                Logger.Error("Failed to load ability balance data for: " + define);
            }

            sb.Clear();
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