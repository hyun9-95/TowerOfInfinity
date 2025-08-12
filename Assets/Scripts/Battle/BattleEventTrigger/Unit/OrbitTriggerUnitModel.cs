public class OrbitTriggerUnitModel : BattleEventTriggerUnitModel
{
    public float OrbitRadius { get; private set; }
    public float StartAngle { get; private set; }

    public float Duration { get; private set; }

    public void SetDuration(float duration)
    {
        Duration = duration;
    }

    public void SetOrbitRadius(float radius)
    {
        OrbitRadius = radius;
    }

    public void SetStartAngle(float angle)
    {
        StartAngle = angle;
    }
}