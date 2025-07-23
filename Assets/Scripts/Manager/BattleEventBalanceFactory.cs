
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

public class BattleEventBalanceFactory : BaseManager<BattleEventBalanceFactory>
{
    private Dictionary<BattleEventBalanceDefine, ScriptableBattleEventBalance> beBalanceDic = new Dictionary<BattleEventBalanceDefine, ScriptableBattleEventBalance>();

    public async UniTask Initialize()
    {
        foreach (BattleEventBalanceDefine define in Enum.GetValues(typeof(BattleEventBalanceDefine)))
        {
            if (define == BattleEventBalanceDefine.None)
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

    public ScriptableBattleEventBalance GetLevelBalance(BattleEventBalanceDefine define)
    {
        if (beBalanceDic.TryGetValue(define, out var levelBalance))
            return levelBalance;

        return null;
    }

    public void Clear()
    {
        beBalanceDic.Clear();
    }
}
