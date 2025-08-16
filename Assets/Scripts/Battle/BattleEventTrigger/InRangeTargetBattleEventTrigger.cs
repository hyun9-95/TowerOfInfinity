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
        var enemiesInRange = GetEnemiesInRange(Model.SpawnCount);
        
        if (enemiesInRange.Count == 0)
            return;
        
        int index = 0;
        foreach (var enemy in enemiesInRange)
        {
            Vector2 targetPosition = enemy.Transform.position;
            
            var colliderTriggerUnit = await SpawnUnitAsync<ColliderTriggerUnit>(Model.TriggerUnitPath, targetPosition, Quaternion.identity);

            if (colliderTriggerUnit != null)
            {
                var model = BattleEventTriggerFactory.CreateColliderUnitModel(Model, null, OnEventHit);
                colliderTriggerUnit.SetModel(model);
                colliderTriggerUnit.ShowAsync().Forget();
            }

            if (Model.SpawnInterval > 0 && index < enemiesInRange.Count - 1)
                await UniTaskUtils.DelaySeconds(Model.SpawnInterval);
            
            index++;
        }
    }

}