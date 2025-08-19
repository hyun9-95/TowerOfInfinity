using System;
using UnityEngine;

public class FollowTargetSupport : MonoBehaviour
{
    [Flags]
    public enum UpdateTiming
    {
        OnEnable = 1 << 0,
        OnFixedUpdate = 1 << 1,
        OnUpdate = 1 << 2,
        OnLateUpdate = 1 << 3,
        OnDisable = 1 << 4
    }

    [SerializeField]
    private Transform target;

    [SerializeField]
    private bool followRotation;

    [SerializeField]
    private UpdateTiming updateTiming = UpdateTiming.OnUpdate;

    private void OnEnable()
    {
        if (updateTiming.HasFlag(UpdateTiming.OnEnable))
            UpdateTransform();
    }

    private void FixedUpdate()
    {
        if (updateTiming.HasFlag(UpdateTiming.OnFixedUpdate))
            UpdateTransform();
    }

    private void Update()
    {
        if (updateTiming.HasFlag(UpdateTiming.OnUpdate))
            UpdateTransform();
    }

    private void LateUpdate()
    {
        if (updateTiming.HasFlag(UpdateTiming.OnLateUpdate))
            UpdateTransform();
    }

    private void OnDisable()
    {
        if (updateTiming.HasFlag(UpdateTiming.OnDisable))
            UpdateTransform();
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    public void SetFollowRotation(bool followRotation)
    {
        this.followRotation = followRotation;
    }

    public void SetTarget(Transform target, bool followRotation)
    {
        this.target = target;
        this.followRotation = followRotation;
    }

    private void UpdateTransform()
    {
        if (target == null)
            return;

        transform.position = target.position;

        if (followRotation)
            transform.rotation = target.rotation;
    }
}