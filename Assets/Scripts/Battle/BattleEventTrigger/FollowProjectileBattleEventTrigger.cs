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
        // 투사체의 타겟 감지 범위는 사거리의 절반
        var enemiesInRange = GetEnemiesInRange();
        
        if (enemiesInRange.Count == 0)
            return;

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
}