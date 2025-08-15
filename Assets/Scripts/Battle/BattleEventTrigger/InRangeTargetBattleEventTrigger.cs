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
        }
    }

}