using UnityEngine;

[CreateAssetMenu(fileName = "AbilityBalance", menuName = "Scriptable Objects/AbilityBalance")]
public class AbilityBalance : ScriptableObject
{
    [SerializeField]
    private float[] values = new float[IntDefine.MAX_ABILITY_LEVEL];

    [SerializeField]
    private float[] speeds = new float[IntDefine.MAX_ABILITY_LEVEL];

    [SerializeField]
    private float[] ranges = new float[IntDefine.MAX_ABILITY_LEVEL];

    [SerializeField]
    private float[] durations = new float[IntDefine.MAX_ABILITY_LEVEL];

    [SerializeField]
    private float[] scales = new float[] { 1, 1, 1, 1, 1 };

    [SerializeField]
    private int[] targetCounts = new int[IntDefine.MAX_ABILITY_LEVEL];

    [SerializeField]
    private float[] coolTimes = new float[IntDefine.MAX_ABILITY_LEVEL];  

    [SerializeField]
    private StatType affectStat = StatType.Attack;

    public float GetValue(int level) => GetByLevel(values, level);
    public float GetSpeed(int level) => GetByLevel(speeds, level);
    public float GetRange(int level) => GetByLevel(ranges, level);
    public float GetScale(int level) => GetByLevel(scales, level);
    public float GetDuration(int level) => GetByLevel(durations, level);
    public int GetTargetCount(int level) => GetByLevel(targetCounts, level);
    public float GetCoolTime(int level) => GetByLevel(coolTimes, level);
    public StatType GetAffectStat() => affectStat;

    private float GetByLevel(float[] array, int level)
    {
        if (level < 0 || level >= array.Length)
        {
            Logger.Error($"[AbilityBalance] Level is bigger than {IntDefine.MAX_ABILITY_LEVEL}");
            return 0f;
        }
        return array[level];
    }

    private int GetByLevel(int[] array, int level)
    {
        if (level < 0 || level >= array.Length)
        {
            Logger.Error($"[AbilityBalance] Level is bigger than {IntDefine.MAX_ABILITY_LEVEL}");
            return 0;
        }
        return array[level];
    }
}
