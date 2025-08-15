#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ProjectileBattleEventTrigger : BattleEventTrigger
{
    protected override async UniTask OnProcess()
    {
        await ProcessProjectile();
    }

    private async UniTask ProcessProjectile()
    {
        for (int i = 0; i < Model.SpawnCount; i++)
        {
            var projectileUnit = await SpawnUnitAsync<ProjectileTriggerUnit>(Model.TriggerUnitPath, Model.Sender.Transform.position, Quaternion.identity);

            if (projectileUnit == null)
                continue;

            var fixedDirection = OnGetFixedDirection(projectileUnit.StartDirectionType);
            var model = BattleEventTriggerFactory.CreateProjectileUnitModel(Model, fixedDirection, null, OnEventHit);
            projectileUnit.SetModel(model);
            projectileUnit.ShowAsync().Forget();
        }
    }
}
