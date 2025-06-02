using System;
using UnityEngine;

public class ProjectileUnitModel : HitTargetUnitModel
{
    public float Speed { get; private set; } = 1f;

    public Vector2 StartDirection { get; private set; }

    public float Distance { get; private set; }

    public float Scale { get; private set; }

    public Vector3 StartPosition { get; private set; }

    public Func<DirectionType, Vector2> OnUpdateDirection { get; private set; }

    public void SetStartPosition(Vector3 pos)
    {
        StartPosition = pos;
    }

    public void SetSpeed(float value)
    {
        Speed = value;
    }

    public void SetScale(float value)
    {
        Scale = value;
    }

    public void SetDirection(Vector3 direction)
    {
        StartDirection = direction;
    }

    public void SetDistance(float value)
    {
        Distance = value;
    }

    public void SetOnUpdateDirection(Func<DirectionType, Vector2> onUpdateDirection)
    {
        OnUpdateDirection = onUpdateDirection;
    }
   
}
