using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class TargetProjectileBattleEventTrigger : BattleEventTrigger
{
    protected override async UniTask OnProcess()
    {
        var enemies = GetEnemiesInRange();
        var usedTargets = new HashSet<CharacterUnitModel>();

        for (int i = 0; i < Model.SpawnCount; i++)
        {
            var projectileUnit = await SpawnUnitAsync<ProjectileTriggerUnit>(Model.PrefabName, Model.Sender.Transform.position, Quaternion.identity);

            if (projectileUnit == null)
                continue;

            Vector2 targetDirection = GetTargetDirection(enemies, usedTargets);
            var model = BattleEventTriggerFactory.CreateProjectileUnitModel(Model, targetDirection, Model.Sender.Transform, OnEventHit);
            projectileUnit.SetModel(model);
            projectileUnit.ShowAsync().Forget();
        }
    }

    private Vector2 GetTargetDirection(List<CharacterUnitModel> enemies, HashSet<CharacterUnitModel> usedTargets)
    {
        CharacterUnitModel selectedTarget = null;

        foreach (var enemy in enemies)
        {
            if (!usedTargets.Contains(enemy))
            {
                selectedTarget = enemy;
                usedTargets.Add(enemy);
                break;
            }
        }

        if (selectedTarget != null)
        {
            Vector2 direction = (selectedTarget.Transform.position - Model.Sender.Transform.position).normalized;
            return direction;
        }

        return GetRandomDirection();
    }
}