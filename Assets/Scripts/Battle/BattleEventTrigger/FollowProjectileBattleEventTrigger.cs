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
        var enemiesInRange = GetEnemiesInRange();
        
        if (enemiesInRange.Count == 0)
        {
            await ProcessRandomDirectionProjectiles();
            return;
        }
        
        int spawnCount = Mathf.Min(Model.SpawnCount, enemiesInRange.Count);
        
        for (int i = 0; i < spawnCount; i++)
        {
            Transform targetEnemy = i < enemiesInRange.Count ? enemiesInRange[i].Transform : null;
            
            var projectileUnit = await SpawnUnitAsync<FollowProjectileTriggerUnit>(Model.PrefabName, Model.Sender.Transform.position, Quaternion.identity);

            if (projectileUnit == null)
                continue;

            var direction = OnGetFixedDirection(DirectionType.Owner);
            var model = BattleEventTriggerFactory.CreateProjectileUnitModel(Model, direction, targetEnemy, OnEventHit);
            projectileUnit.SetModel(model);
            projectileUnit.ShowAsync().Forget();
        }
    }

    private async UniTask ProcessRandomDirectionProjectiles()
    {
        for (int i = 0; i < Model.SpawnCount; i++)
        {
            var projectileUnit = await SpawnUnitAsync<ProjectileTriggerUnit>(Model.PrefabName, Model.Sender.Transform.position, Quaternion.identity);

            if (projectileUnit == null)
                continue;

            var model = BattleEventTriggerFactory.CreateProjectileUnitModel(Model, GetRandomDirection(), null, OnEventHit);
            projectileUnit.SetModel(model);
            projectileUnit.ShowAsync().Forget();
        }
    }

}