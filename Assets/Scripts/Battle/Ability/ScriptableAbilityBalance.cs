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
    private int[] TargetCount;

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
            return TargetCount[TargetCount.Length - 1];
        return TargetCount[level];
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
        TargetCount = new int[IntDefine.MAX_ABILITY_LEVEL];
        Scale = new float[IntDefine.MAX_ABILITY_LEVEL];
        CoolTime = new float[IntDefine.MAX_ABILITY_LEVEL];

        for (int i = 0; i < IntDefine.MAX_ABILITY_LEVEL; i++)
        {
            Speed[i] = 0f;
            Range[i] = 0f;
            Duration[i] = 0f;
            TargetCount[i] = 0;
            Scale[i] = 0f;
            CoolTime[i] = 0f;
        }
    }

    public void DeepCopy(ScriptableAbilityBalance source)
    {
        Speed = source.Speed != null ? (float[])source.Speed.Clone() : null;
        Range = source.Range != null ? (float[])source.Range.Clone() : null;
        Duration = source.Duration != null ? (float[])source.Duration.Clone() : null;
        TargetCount = source.TargetCount != null ? (int[])source.TargetCount.Clone() : null;
        Scale = source.Scale != null ? (float[])source.Scale.Clone() : null;
        CoolTime = source.CoolTime != null ? (float[])source.CoolTime.Clone() : null;
    }
}