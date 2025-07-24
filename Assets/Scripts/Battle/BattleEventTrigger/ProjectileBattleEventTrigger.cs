#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ProjectileBattleEventTrigger : BattleEventTrigger
{
    public override async UniTask Process()
    {
        await ProcessProjectile();
    }

    private async UniTask ProcessProjectile()
    {
        var projectileUnit = await SpawnUnitAsync<ProjectileTriggerUnit>(Model.PrefabName, Model.Sender.Transform.position, Quaternion.identity);

        if (projectileUnit == null)
            return;

        if (projectileUnit.Model == null)
            projectileUnit.SetModel(new ProjectileTriggerUnitModel());

        var projectileUnitModel = projectileUnit.Model;
        var fixedDirection = OnGetFixedDirection(projectileUnit.DirectionType);

        projectileUnitModel.SetDirection(fixedDirection);
        projectileUnitModel.SetDistance(Model.Range);
        projectileUnitModel.SetSpeed(Model.Speed);
        projectileUnitModel.SetScale(Model.Scale);
        projectileUnitModel.SetStartPosition(Model.Sender.Transform.position);
        projectileUnitModel.SetOnEventHit(OnEventHit);
        projectileUnit.ShowAsync().Forget();
    }
}
