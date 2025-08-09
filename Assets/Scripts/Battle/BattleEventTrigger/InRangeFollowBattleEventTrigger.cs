using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class InRangeFollowBattleEventTrigger : BattleEventTrigger
{
    protected override async UniTask OnProcess()
    {
        await ProcessInRangeFollowEvent();
    }

    private async UniTask ProcessInRangeFollowEvent()
    {
        var enemiesInRange = GetEnemyTransformsInRange();
        
        if (enemiesInRange.Count == 0)
            return;

        int spawnCount = Mathf.Min(Model.SpawnCount, enemiesInRange.Count);
        
        for (int i = 0; i < spawnCount; i++)
        {
            Transform targetEnemy = enemiesInRange[i];
            
            var colliderTriggerUnit = await SpawnUnitAsync<ColliderTriggerUnit>(Model.PrefabName, Model.Sender.Transform.position, Quaternion.identity);

            if (colliderTriggerUnit != null)
            {
                var model = BattleEventTriggerFactory.CreateColliderUnitModel(Model, targetEnemy, OnEventHit);
                colliderTriggerUnit.SetModel(model);
                colliderTriggerUnit.ShowAsync().Forget();
            }
        }
    }
}