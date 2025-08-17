using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Balance/Ability Balance")]
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
    private int[] HitCount;

    [SerializeField]
    private float[] Scale;

    [SerializeField]
    private float[] CoolTime;

    [SerializeField]
    private int[] SpawnCount;

    [SerializeField]
    private float[] HitForce;

    [SerializeField]
    private float[] SpawnInterval;

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

    public int GetHitCount(int level)
    {
        if (HitCount == null || HitCount.Length == 0)
            return 0;

        if (level >= HitCount.Length)
            return HitCount[HitCount.Length - 1];

        return HitCount[level];
    }

    public float GetCoolTime(int level)
    {
        if (CoolTime == null || CoolTime.Length == 0)
            return 0f;

        if (level >= CoolTime.Length)
            return CoolTime[CoolTime.Length - 1];

        return CoolTime[level];
    }

    public int GetSpawnCount(int level)
    {
        if (SpawnCount == null || SpawnCount.Length == 0)
            return 1;

        if (level >= SpawnCount.Length)
            return SpawnCount[SpawnCount.Length - 1];

        return SpawnCount[level];
    }

    public float GetHitForce(int level)
    {
        if (HitForce == null || HitForce.Length == 0)
            return 0;

        if (level >= HitForce.Length)
            return HitForce[HitForce.Length - 1];

        return HitForce[level];
    }

    public float GetSpawnInterval(int level)
    {
        if (SpawnInterval == null || SpawnInterval.Length == 0)
            return 0f;

        if (level >= SpawnInterval.Length)
            return SpawnInterval[SpawnInterval.Length - 1];

        return SpawnInterval[level];
    }

    public void ResetBalanceValues()
    {
        Speed = new float[0];
        Range = new float[0];
        Duration = new float[0];
        HitCount = new int[0];
        Scale = new float[0];
        CoolTime = new float[0];
        SpawnCount = new int[0];
        HitForce = new float[0];
        SpawnInterval = new float[0];
    }

    public void DeepCopy(ScriptableAbilityBalance source)
    {
        Speed = source.Speed != null ? (float[])source.Speed.Clone() : null;
        Range = source.Range != null ? (float[])source.Range.Clone() : null;
        Duration = source.Duration != null ? (float[])source.Duration.Clone() : null;
        HitCount = source.HitCount != null ? (int[])source.HitCount.Clone() : null;
        Scale = source.Scale != null ? (float[])source.Scale.Clone() : null;
        CoolTime = source.CoolTime != null ? (float[])source.CoolTime.Clone() : null;
        SpawnCount = source.SpawnCount != null ? (int[])source.SpawnCount.Clone() : null;
        HitForce = source.HitForce != null ? (float[])source.HitForce.Clone() : null;
        SpawnInterval = source.SpawnInterval != null ? (float[])source.SpawnInterval.Clone() : null;
    }
}