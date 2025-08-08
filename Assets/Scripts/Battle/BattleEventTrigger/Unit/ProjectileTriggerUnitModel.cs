using System;
using UnityEngine;

public class ProjectileTriggerUnitModel : BattleEventTriggerUnitModel
{
    public float Speed { get; private set; } = 1f;

    public Vector2 StartDirection { get; private set; }

    public float Distance { get; private set; }

    public Vector3 StartPosition { get; private set; }

    public void SetStartPosition(Vector3 pos)
    {
        StartPosition = pos;
    }

    public void SetSpeed(float value)
    {
        Speed = value;
    }

    public void SetDirection(Vector3 direction)
    {
        StartDirection = direction;
    }

    public void SetDistance(float value)
    {
        Distance = value;
    }
}
