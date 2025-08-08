#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBattleEventTrigger : BattleEventTrigger
{
    private List<ProjectileTriggerUnit> spawnedProjectiles = new List<ProjectileTriggerUnit>();
    
    public override async UniTask Process()
    {
        await ProcessProjectile();
    }

    protected override void OnComplete()
    {
        DeactivateAllProjectiles();
        base.OnComplete();
    }

    private void DeactivateAllProjectiles()
    {
        foreach (var projectile in spawnedProjectiles)
        {
            if (projectile != null && projectile.gameObject.activeSelf)
                projectile.Deactivate();
        }
        spawnedProjectiles.Clear();
    }

    private async UniTask ProcessProjectile()
    {
        int spawnCount = Model.SpawnCount;
        
        for (int i = 0; i < spawnCount; i++)
        {
            var projectileUnit = await SpawnUnitAsync<ProjectileTriggerUnit>(Model.PrefabName, Model.Sender.Transform.position, Quaternion.identity);

            if (projectileUnit == null)
                continue;

            spawnedProjectiles.Add(projectileUnit);

            if (projectileUnit.Model == null)
                projectileUnit.SetModel(new ProjectileTriggerUnitModel());

            var projectileUnitModel = projectileUnit.Model;
            var fixedDirection = OnGetFixedDirection(projectileUnit.StartDirectionType);

            projectileUnitModel.SetDirection(fixedDirection);
            projectileUnitModel.SetDistance(Model.Range);
            projectileUnitModel.SetSpeed(Model.Speed);
            projectileUnitModel.SetStartPosition(Model.Sender.Transform.position);
            projectileUnitModel.SetOnEventHit(OnEventHit);
            projectileUnitModel.SetDetectTeamTag(Model.Sender.TeamTag.Opposite());
            projectileUnit.ShowAsync().Forget();
        }
    }
}
