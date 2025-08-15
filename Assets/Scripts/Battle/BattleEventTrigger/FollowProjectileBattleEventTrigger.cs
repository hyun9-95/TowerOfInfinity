#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using UnityEngine;

public class FollowProjectileBattleEventTrigger : BattleEventTrigger
{
    protected override async UniTask OnProcess()
    {
        await ProcessFollowProjectile();
    }

    private async UniTask ProcessFollowProjectile()
    {
        var enemiesInRange = GetEnemiesInRange(Model.SpawnCount);
        
        if (enemiesInRange.Count == 0)
            return;

        foreach (var enemy in enemiesInRange)
        {
            Transform targetEnemy = enemy.Transform;
            
            var projectileUnit = await SpawnUnitAsync<FollowProjectileTriggerUnit>(Model.TriggerUnitPath, Model.Sender.Transform.position, Quaternion.identity);

            if (projectileUnit == null)
                continue;

            var direction = OnGetFixedDirection(DirectionType.Owner);
            var model = BattleEventTriggerFactory.CreateProjectileUnitModel(Model, direction, targetEnemy, OnEventHit);
            projectileUnit.SetModel(model);
            projectileUnit.ShowAsync().Forget();
        }
    }
}