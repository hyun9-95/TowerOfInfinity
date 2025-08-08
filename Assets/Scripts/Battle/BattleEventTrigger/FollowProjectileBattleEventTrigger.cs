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
            return;

        int spawnCount = Mathf.Min(Model.SpawnCount, enemiesInRange.Count);
        
        for (int i = 0; i < spawnCount; i++)
        {
            Transform targetEnemy = enemiesInRange[i].Transform;
            
            var projectileUnit = await SpawnUnitAsync<FollowProjectileTriggerUnit>(Model.PrefabName, Model.Sender.Transform.position, Quaternion.identity);

            if (projectileUnit == null)
                continue;

            if (projectileUnit.Model == null)
                projectileUnit.SetModel(new ProjectileTriggerUnitModel());

            var projectileUnitModel = projectileUnit.Model;
            projectileUnitModel.SetFollowTarget(targetEnemy);
            projectileUnitModel.SetDistance(Model.Range);
            projectileUnitModel.SetSpeed(Model.Speed);
            projectileUnitModel.SetStartPosition(Model.Sender.Transform.position);
            projectileUnitModel.SetOnEventHit(OnEventHit);
            projectileUnitModel.SetDetectTeamTag(Model.Sender.TeamTag.Opposite());
            projectileUnitModel.SetHitCount(Model.HitCount);
            
            projectileUnit.ShowAsync().Forget();
        }
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