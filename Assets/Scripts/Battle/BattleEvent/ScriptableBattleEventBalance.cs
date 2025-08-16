
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableBattleEventBalance")]
public class ScriptableBattleEventBalance : ScriptableObject
{
    [SerializeField]
    private BattleEventDefine define;
    public BattleEventDefine Define => define;

    public void SetType(BattleEventDefine newType)
    {
        define = newType;
    }

    [SerializeField]
    private float[] value;

    [SerializeField]
    private float[] value2;

    [SerializeField]
    private float[] duration;

    [SerializeField]
    private float[] applyIntervalSeconds;

    public float GetDuration(int level)
    {
        if (duration == null || duration.Length == 0)
            return 0f;
        if (level >= duration.Length)
            return duration[duration.Length - 1];
        return duration[level];
    }

    public float GetValue(int level)
    {
        if (value == null || value.Length == 0)
            return 0f;
        if (level >= value.Length)
            return value[value.Length - 1];
        return value[level];
    }

    public float GetValue2(int level)
    {
        if (value2 == null || value2.Length == 0)
            return 0f;
        if (level >= value2.Length)
            return value2[value2.Length - 1];
        return value2[level];
    }

    public float GetApplyIntervalSeconds(int level)
    {
        if (applyIntervalSeconds == null || applyIntervalSeconds.Length == 0)
            return 0f;
        if (level >= applyIntervalSeconds.Length)
            return applyIntervalSeconds[applyIntervalSeconds.Length - 1];
        return applyIntervalSeconds[level];
    }

    public void ResetBalanceValues()
    {
        duration = new float[0];
        value = new float[0];
        value2 = new float[0];
        applyIntervalSeconds = new float[0];
    }

    public void DeepCopy(ScriptableBattleEventBalance source)
    {
        duration = source.duration != null ? (float[])source.duration.Clone() : null;
        value = source.value != null ? (float[])source.value.Clone() : null;
        value2 = source.value2 != null ? (float[])source.value2.Clone() : null;
        applyIntervalSeconds = source.applyIntervalSeconds != null ? (float[])source.applyIntervalSeconds.Clone() : null;
    }
}
