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
            BattleEventTriggerType.FrontAngleProjectile => new FrontAngleProjectileBattleEventTrigger(),
            BattleEventTriggerType.Self => selfBattleEventTrigger,
            BattleEventTriggerType.Orbit => new OrbitBattleEventTrigger(),
            BattleEventTriggerType.TargetProjectile => new TargetProjectileBattleEventTrigger(),
            BattleEventTriggerType.InRangeRandom => new InRangeRandomBattleEventTrigger(),
            BattleEventTriggerType.Blackhole => new BlackholeBattleEventTrigger(),
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
        {
            var targetModel = BattleApi.OnGetAliveCharModel(followTarget);

            if (targetModel != null)
                projectileUnitModel.SetFollowTargetModel(targetModel);
        }

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
        {
            var targetModel = BattleApi.OnGetAliveCharModel(followTarget);

            if (targetModel != null)
                colliderUnitModel.SetFollowTargetModel(targetModel);
        }

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
        {
            var targetModel = BattleApi.OnGetAliveCharModel(followTarget);

            if (targetModel != null)
                orbitUnitModel.SetFollowTargetModel(targetModel);
        }

        return orbitUnitModel;
    }

    public static BlackholeTriggerUnitModel CreateBlackholeUnitModel(
        BattleEventTriggerModel triggerModel,
        Func<Collider2D, Vector3, bool> onEventHit = null)
    {
        var blackholeUnitModel = new BlackholeTriggerUnitModel();
        blackholeUnitModel.SetDetectTeamTag(triggerModel.TargetTeamTag);
        blackholeUnitModel.SetOnEventHit(onEventHit);
        blackholeUnitModel.SetHitCount(triggerModel.HitCount);
        blackholeUnitModel.SetPullForce(triggerModel.HitForce);

        return blackholeUnitModel;
    }
}
