using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableAbilityEventBalance")]
public class ScriptableAbilityBalance : ScriptableObject
{
    [SerializeField]
    private AbilityDefine type;
    public AbilityDefine Type => type;

    public void SetType(AbilityDefine newType)
    {
        type = newType;
    }

    [SerializeField]
    private float[] Speed;

    [SerializeField]
    private float[] Range;

    [SerializeField]
    private float[] Duration;

    [SerializeField]
    private float[] TargetCount;

    [SerializeField]
    private float[] Scale;

    [SerializeField]
    private float[] CoolTime;

    public float GetSpeed(int level)
    {
        if (Speed == null || Speed.Length <= level)
            return 0f;
        return Speed[level];
    }
    public float GetRange(int level)
    {
        if (Range == null || Range.Length <= level)
            return 0f;
        return Range[level];
    }
    public float GetDuration(int level)
    {
        if (Duration == null || Duration.Length <= level)
            return 0f;
        return Duration[level];
    }
    public float GetScale(int level)
    {
        if (Scale == null || Scale.Length <= level)
            return 0f;
        return Scale[level];
    }

    public int GetTargetCount(int level)
    {
        if (TargetCount == null || TargetCount.Length <= level)
            return 0;
        return (int)TargetCount[level];
    }

    public float GetCoolTime(int level)
    {
        if (CoolTime == null || CoolTime.Length <= level)
            return 0f;
        return CoolTime[level];
    }
}