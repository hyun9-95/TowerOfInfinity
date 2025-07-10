using UnityEngine;

public class HitTargetRangeUnitModel : HitTargetUnitModel
{
    public float Range { get; private set; }

    public bool IsFlip { get; private set; }

    public void SetRange(float range)
    {
        Range = range;
    }

    public void SetFlip(bool flip)
    {
        IsFlip = flip;
    }
}
