using System;
using UnityEngine;

public static class BattleEventTriggerFactory
{
    private static SelfBattleEventTrigger selfBattleEventTrigger = new();

    public static BattleEventTrigger Create(BattleEventTriggerType type)
    {
        BattleEventTrigger newTrigger = type switch
        {
            BattleEventTriggerType.Projectile => new ProjectileBattleEventTrigger(),
            BattleEventTriggerType.FollowCollider => new FollowColliderBattleEventTrigger(),
            BattleEventTriggerType.InRange => new InRangeTargetBattleEventTrigger(),
            BattleEventTriggerType.InRangeFollow => new InRangeFollowBattleEventTrigger(),
            BattleEventTriggerType.FollowProjectile => new FollowProjectileBattleEventTrigger(),
            BattleEventTriggerType.RandomProjectile => new RandomProjectileBattleEventTrigger(),
            BattleEventTriggerType.Self => selfBattleEventTrigger,
            BattleEventTriggerType.Orbit => new OrbitBattleEventTrigger(),
            _ => null
        };

        return newTrigger;
    }

    public static ProjectileTriggerUnitModel CreateProjectileUnitModel(
        BattleEventTriggerModel triggerModel, 
        Vector2 direction, 
        Transform followTarget = null,
        Func<Collider2D, Vector3, bool> onEventHit = null)
    {
        var projectileUnitModel = new ProjectileTriggerUnitModel();
        projectileUnitModel.SetDirection(direction);
        projectileUnitModel.SetMoveDistance(triggerModel.Range);
        projectileUnitModel.SetSpeed(triggerModel.Speed);
        projectileUnitModel.SetStartPosition(triggerModel.Sender.Transform.position);
        projectileUnitModel.SetOnEventHit(onEventHit);
        projectileUnitModel.SetDetectTeamTag(triggerModel.TargetTeamTag);
        projectileUnitModel.SetHitCount(triggerModel.HitCount);

        if (followTarget != null)
            projectileUnitModel.SetFollowTarget(followTarget);

        return projectileUnitModel;
    }

    public static RangeTriggerUnitModel CreateColliderUnitModel(
        BattleEventTriggerModel triggerModel,
        Transform followTarget = null,
        Func<Collider2D, Vector3, bool> onEventHit = null)
    {
        var colliderUnitModel = new RangeTriggerUnitModel();
        colliderUnitModel.SetFlip(triggerModel.Sender.IsFlipX);
        colliderUnitModel.SetDetectTeamTag(triggerModel.TargetTeamTag);
        colliderUnitModel.SetOnEventHit(onEventHit);
        colliderUnitModel.SetHitCount(triggerModel.HitCount);

        if (followTarget != null)
            colliderUnitModel.SetFollowTarget(followTarget);

        return colliderUnitModel;
    }

    public static OrbitTriggerUnitModel CreateOrbitUnitModel(
        BattleEventTriggerModel triggerModel,
        float startAngle,
        Transform followTarget = null,
        Func<Collider2D, Vector3, bool> onEventHit = null)
    {
        var orbitUnitModel = new OrbitTriggerUnitModel();
        orbitUnitModel.SetFlip(triggerModel.Sender.IsFlipX);
        orbitUnitModel.SetDetectTeamTag(triggerModel.TargetTeamTag);
        orbitUnitModel.SetOnEventHit(onEventHit);
        orbitUnitModel.SetHitCount(triggerModel.HitCount);
        orbitUnitModel.SetOrbitRadius(triggerModel.Range);
        orbitUnitModel.SetDuration(triggerModel.Duration);
        orbitUnitModel.SetStartAngle(startAngle);

        if (followTarget != null)
            orbitUnitModel.SetFollowTarget(followTarget);

        return orbitUnitModel;
    }
}
