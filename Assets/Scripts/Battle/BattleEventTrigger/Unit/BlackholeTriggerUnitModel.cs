using Cysharp.Threading.Tasks;
using UnityEngine;

public class BlackholeTriggerUnitModel : BattleEventTriggerUnitModel
{
    public float PullForce { get; private set; }

    public void SetPullForce(float force)
    {
        PullForce = force;
    }

    public override void OnEventHit(Collider2D collider, Vector3 hitPos)
    {
        if (IsOverHitCount())
            return;

        base.OnEventHit(collider, hitPos);

        if (PullForce > 0)
        {
            var targetModel = BattleSceneManager.GetAliveCharModel(collider);
            if (targetModel != null && targetModel.TeamTag == DetectTeamTag)
            {
                Vector2 direction = (hitPos - collider.transform.position).normalized;
                targetModel.ActionHandler.OnAddForceAsync(direction, PullForce).Forget();
            }
        }
    }
}