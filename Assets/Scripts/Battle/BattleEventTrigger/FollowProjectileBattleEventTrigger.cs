#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
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
            Transform targetEnemy = i < enemiesInRange.Count ?
                enemiesInRange[i].Transform : null;
            
            var projectileUnit = await SpawnUnitAsync<FollowProjectileTriggerUnit>(Model.PrefabName, Model.Sender.Transform.position, Quaternion.identity);

            if (projectileUnit == null)
                continue;

            if (projectileUnit.Model == null)
                projectileUnit.SetModel(new ProjectileTriggerUnitModel());

            var projectileUnitModel = projectileUnit.Model;
            projectileUnitModel.SetFollowTarget(targetEnemy);
            projectileUnitModel.SetMoveDistance(Model.Range);
            projectileUnitModel.SetSpeed(Model.Speed);
            projectileUnitModel.SetStartPosition(Model.Sender.Transform.position);
            projectileUnitModel.SetDirection(OnGetFixedDirection(DirectionType.Owner));
            projectileUnitModel.SetOnEventHit(OnEventHit);
            projectileUnitModel.SetDetectTeamTag(Model.Sender.TeamTag.Opposite());
            projectileUnitModel.SetHitCount(Model.HitCount);
            
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

            if (projectileUnit.Model == null)
                projectileUnit.SetModel(new ProjectileTriggerUnitModel());

            var projectileUnitModel = projectileUnit.Model;
            Vector2 randomDirection = GetRandomDirection();
            
            projectileUnitModel.SetDirection(randomDirection);
            projectileUnitModel.SetMoveDistance(Model.Range);
            projectileUnitModel.SetSpeed(Model.Speed);
            projectileUnitModel.SetStartPosition(Model.Sender.Transform.position);
            projectileUnitModel.SetOnEventHit(OnEventHit);
            projectileUnitModel.SetDetectTeamTag(Model.Sender.TeamTag.Opposite());
            projectileUnitModel.SetHitCount(Model.HitCount);
            
            projectileUnit.ShowAsync().Forget();
        }
    }

    private Vector2 GetRandomDirection()
    {
        float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle));
    }

    private List<CharacterUnitModel> GetEnemiesInRange()
    {
        Vector2 senderPosition = Model.Sender.Transform.position;
        var colliders = Physics2D.OverlapCircleAll(senderPosition, Model.Range, (int)LayerFlag.Character);
        var enemies = new List<CharacterUnitModel>();
        
        colliders.SortByNearest(senderPosition);
        
        foreach (var collider in colliders)
        {
            var targetModel = BattleSceneManager.Instance.GetCharacterModel(collider);

            if (targetModel == null || targetModel.TeamTag == Model.Sender.TeamTag)
                continue;

            if (targetModel == Model.Sender)
                continue;

            enemies.Add(targetModel);
        }
        
        return enemies;
    }
}