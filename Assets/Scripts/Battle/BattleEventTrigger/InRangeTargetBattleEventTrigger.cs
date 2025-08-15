using Cysharp.Threading.Tasks;
using UnityEngine;

public class InRangeTargetBattleEventTrigger : BattleEventTrigger
{
    protected override async UniTask OnProcess()
    {
        await ProcessRandomRangeEvent();
    }

    private async UniTask ProcessRandomRangeEvent()
    {
        var enemiesInRange = GetEnemiesInRange();
        
        if (enemiesInRange.Count == 0)
            return;
        
        int spawnCount = Mathf.Min(Model.SpawnCount, enemiesInRange.Count);
        
        for (int i = 0; i < spawnCount; i++)
        {
            Vector2 targetPosition = enemiesInRange[i].Transform.position;
            
            var colliderTriggerUnit = await SpawnUnitAsync<ColliderTriggerUnit>(Model.TriggerUnitPath, targetPosition, Quaternion.identity);

            if (colliderTriggerUnit != null)
            {
                var model = BattleEventTriggerFactory.CreateColliderUnitModel(Model, null, OnEventHit);
                colliderTriggerUnit.SetModel(model);
                colliderTriggerUnit.ShowAsync().Forget();
            }
        }
    }

}