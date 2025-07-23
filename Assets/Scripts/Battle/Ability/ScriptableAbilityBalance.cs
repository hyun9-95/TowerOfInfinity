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
        if (Speed == null || Speed.Length == 0)
            return 0f;
        if (level >= Speed.Length)
            return Speed[Speed.Length - 1];
        return Speed[level];
    }
    public float GetRange(int level)
    {
        if (Range == null || Range.Length == 0)
            return 0f;
        if (level >= Range.Length)
            return Range[Range.Length - 1];
        return Range[level];
    }
    public float GetDuration(int level)
    {
        if (Duration == null || Duration.Length == 0)
            return 0f;
        if (level >= Duration.Length)
            return Duration[Duration.Length - 1];
        return Duration[level];
    }
    public float GetScale(int level)
    {
        if (Scale == null || Scale.Length == 0)
            return 0f;
        if (level >= Scale.Length)
            return Scale[Scale.Length - 1];
        return Scale[level];
    }

    public int GetTargetCount(int level)
    {
        if (TargetCount == null || TargetCount.Length == 0)
            return 0;
        if (level >= TargetCount.Length)
            return (int)TargetCount[TargetCount.Length - 1];
        return (int)TargetCount[level];
    }

    public float GetCoolTime(int level)
    {
        if (CoolTime == null || CoolTime.Length == 0)
            return 0f;
        if (level >= CoolTime.Length)
            return CoolTime[CoolTime.Length - 1];
        return CoolTime[level];
    }

    public void ResetBalanceValues()
    {
        Speed = new float[IntDefine.MAX_ABILITY_LEVEL];
        Range = new float[IntDefine.MAX_ABILITY_LEVEL];
        Duration = new float[IntDefine.MAX_ABILITY_LEVEL];
        TargetCount = new float[IntDefine.MAX_ABILITY_LEVEL];
        Scale = new float[IntDefine.MAX_ABILITY_LEVEL];
        CoolTime = new float[IntDefine.MAX_ABILITY_LEVEL];

        for (int i = 0; i < IntDefine.MAX_ABILITY_LEVEL; i++)
        {
            Speed[i] = 0f;
            Range[i] = 0f;
            Duration[i] = 0f;
            TargetCount[i] = 0f;
            Scale[i] = 0f;
            CoolTime[i] = 0f;
        }
    }
}