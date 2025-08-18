using System;
using UnityEngine;

public class ProjectileTriggerUnitModel : BattleEventTriggerUnitModel
{
    public float Speed { get; private set; } = 1f;

    public float MoveDistance { get; private set; }

    public Vector3 StartPosition { get; private set; }

    public void SetStartPosition(Vector3 pos)
    {
        StartPosition = pos;
    }

    public void SetSpeed(float value)
    {
        Speed = value;
    }

    public void SetMoveDistance(float value)
    {
        MoveDistance = value;
    }
}
