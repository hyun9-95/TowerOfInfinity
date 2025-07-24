
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
        duration = new float[IntDefine.MAX_ABILITY_LEVEL];
        value = new float[IntDefine.MAX_ABILITY_LEVEL];
        applyIntervalSeconds = new float[IntDefine.MAX_ABILITY_LEVEL];

        for (int i = 0; i < IntDefine.MAX_ABILITY_LEVEL; i++)
        {
            duration[i] = 0f;
            value[i] = 0f;
            applyIntervalSeconds[i] = 0f;
        }
    }

    public void DeepCopy(ScriptableBattleEventBalance source)
    {
        duration = source.duration != null ? (float[])source.duration.Clone() : null;
        value = source.value != null ? (float[])source.value.Clone() : null;
        applyIntervalSeconds = source.applyIntervalSeconds != null ? (float[])source.applyIntervalSeconds.Clone() : null;
    }
}
