#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBattleEventTrigger : BattleEventTrigger
{
    protected override async UniTask OnProcess()
    {
        await ProcessProjectile();
    }

    private async UniTask ProcessProjectile()
    {
        int spawnCount = Model.SpawnCount;
        
        for (int i = 0; i < spawnCount; i++)
        {
            var projectileUnit = await SpawnUnitAsync<ProjectileTriggerUnit>(Model.PrefabName, Model.Sender.Transform.position, Quaternion.identity);

            if (projectileUnit == null)
                continue;

            if (projectileUnit.Model == null)
                projectileUnit.SetModel(new ProjectileTriggerUnitModel());

            var projectileUnitModel = projectileUnit.Model;
            var fixedDirection = OnGetFixedDirection(projectileUnit.StartDirectionType);

            projectileUnitModel.SetDirection(fixedDirection);
            projectileUnitModel.SetMoveDistance(Model.Range);
            projectileUnitModel.SetSpeed(Model.Speed);
            projectileUnitModel.SetStartPosition(Model.Sender.Transform.position);
            projectileUnitModel.SetOnEventHit(OnEventHit);
            projectileUnitModel.SetDetectTeamTag(Model.Sender.TeamTag.Opposite());
            projectileUnitModel.SetHitCount(Model.HitCount);
            projectileUnit.ShowAsync().Forget();
        }
    }
}
