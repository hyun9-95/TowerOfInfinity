#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using UnityEngine;

public class OrbitTriggerUnit : BaseTriggerUnit<OrbitTriggerUnitModel>
{
    // 초당 1바퀴
    private float orbitSpeed = 360f;
    
    private float currentAngle;
    private Transform followTarget;
    private Vector3 centerOffset;

    #region Lifecycle
    protected override void OnUnitAwake()
    {
        followTarget = null;
    }
    
    public override async UniTask ShowAsync()
    {
        if (Model == null)
            return;

        AddEnemyKilledObserver();

        if (Model.FollowTarget != null)
        {
            followTarget = Model.FollowTarget;
            centerOffset = Vector3.zero;
        }

        currentAngle = Model.StartAngle;

        ShowRenderer(fadeInTime);
        hitCollider.enabled = true;
        
        StartOrbitMovement();

        // 지속시간 종료 후 페이드아웃하면서 꺼짐
        UniTaskUtils.DelayAction(Model.Duration, DeactivateWithFade, TokenPool.Get(GetHashCode())).Forget();
    }
    #endregion

    private void StartOrbitMovement()
    {
        if (followTarget != null)
            UpdateOrbitMovement().Forget();
    }

    private async UniTask UpdateOrbitMovement()
    {
        while (!gameObject.CheckSafeNull() && gameObject.activeSelf && followTarget != null)
        {
            currentAngle += orbitSpeed * Time.fixedDeltaTime;
            if (currentAngle >= 360f)
                currentAngle -= 360f;

            float radians = currentAngle * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(
                Mathf.Cos(radians) * Model.OrbitRadius,
                Mathf.Sin(radians) * Model.OrbitRadius,
                0f
            );

            transform.position = followTarget.position + centerOffset + offset;
            
            await UniTask.NextFrame(PlayerLoopTiming.FixedUpdate, TokenPool.Get(GetHashCode()));
        }
    }
}