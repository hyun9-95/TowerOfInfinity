using UnityEngine;

public class HitTargetRangeUnitModel : HitTargetUnitModel
{
    public float Range { get; private set; }

    public void SetRange(float range)
    {
        Range = range;
    }
}
