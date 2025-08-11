
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Text;

public class BattleEventBalanceFactory : BaseManager<BattleEventBalanceFactory>
{
    private Dictionary<BattleEventDefine, ScriptableBattleEventBalance> beBalanceDic = new Dictionary<BattleEventDefine, ScriptableBattleEventBalance>();
    private string folderPath = "AbilityCore/BattleEventBalance/";
    public async UniTask Initialize()
    {
        StringBuilder sb = GlobalStringBuilder.Get();
        foreach (BattleEventDefine define in Enum.GetValues(typeof(BattleEventDefine)))
        {
            if (define == BattleEventDefine.None)
                continue;

            var eventPath = sb.Append(folderPath).Append(define.ToString()).ToString();
            var levelBalance = await AddressableManager.Instance.LoadScriptableObject<ScriptableBattleEventBalance>(eventPath, false);
            
            if (levelBalance != null)
            {
                beBalanceDic[define] = levelBalance;
            }
            else
            {
                Logger.Error("Failed to load level balance data for: " + define);
            }

            sb.Clear();
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
