
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

public class BattleEventBalanceFactory : BaseManager<BattleEventBalanceFactory>
{
    private Dictionary<BattleEventDefine, ScriptableBattleEventBalance> beBalanceDic = new Dictionary<BattleEventDefine, ScriptableBattleEventBalance>();

    public async UniTask Initialize()
    {
        foreach (BattleEventDefine define in Enum.GetValues(typeof(BattleEventDefine)))
        {
            if (define == BattleEventDefine.None)
                continue;

            var levelBalance = await AddressableManager.Instance.LoadScriptableObject<ScriptableBattleEventBalance>(define.ToString(), false);
            
            if (levelBalance != null)
            {
                beBalanceDic[define] = levelBalance;
            }
            else
            {
                Logger.Error("Failed to load level balance data for: " + define);
            }
        }
    }

    public ScriptableBattleEventBalance GetLevelBalance(int id)
    {
        BattleEventDefine define = (BattleEventDefine)id;
        
        if (beBalanceDic.TryGetValue(define, out var levelBalance))
            return levelBalance;

        return null;
    }

    public void Clear()
    {
        beBalanceDic.Clear();
    }
}
