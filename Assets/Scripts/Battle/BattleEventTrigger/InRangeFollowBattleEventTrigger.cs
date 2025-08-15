using Cysharp.Threading.Tasks;
using UnityEngine;

public class InRangeFollowBattleEventTrigger : BattleEventTrigger
{
    protected override async UniTask OnProcess()
    {
        await ProcessInRangeFollowEvent();
    }

    private async UniTask ProcessInRangeFollowEvent()
    {
        var enemiesInRange = GetEnemiesInRange(Model.SpawnCount);
        
        if (enemiesInRange.Count == 0)
            return;

        foreach (var enemy in enemiesInRange)
        {
            Transform targetEnemy = enemy.Transform;
            
            var colliderTriggerUnit = await SpawnUnitAsync<ColliderTriggerUnit>(Model.TriggerUnitPath, Model.Sender.Transform.position, Quaternion.identity);

            if (colliderTriggerUnit != null)
            {
                var model = BattleEventTriggerFactory.CreateColliderUnitModel(Model, targetEnemy, OnEventHit);
                colliderTriggerUnit.SetModel(model);
                colliderTriggerUnit.ShowAsync().Forget();
            }
        }
    }
}