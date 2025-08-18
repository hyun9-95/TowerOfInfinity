#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using UnityEngine;

public class RandomProjectileBattleEventTrigger : BattleEventTrigger
{
    protected override async UniTask OnProcess()
    {
        await ProcessRandomProjectiles();
    }

    private async UniTask ProcessRandomProjectiles()
    {
        for (int i = 0; i < Model.SpawnCount; i++)
        {
            var projectileUnit = await SpawnUnitAsync<ProjectileTriggerUnit>(Model.TriggerUnitPath, Model.Sender.Transform.position, Quaternion.identity);

            if (projectileUnit == null)
                continue;
            
            var model = BattleEventTriggerFactory.CreateProjectileUnitModel(Model, GetRandomDirection(), null, OnEventHit);
            projectileUnit.SetModel(model);
            projectileUnit.ShowAsync().Forget();

            if (Model.SpawnInterval > 0 && i < Model.SpawnCount - 1)
                await UniTaskUtils.DelaySeconds(Model.SpawnInterval);
        }
    }
}