using UnityEngine;

public class RangeTriggerUnitModel : BattleEventTriggerUnitModel
{
    public float Range { get; private set; }

    public void SetRange(float range)
    {
        Range = range;
    }
}
