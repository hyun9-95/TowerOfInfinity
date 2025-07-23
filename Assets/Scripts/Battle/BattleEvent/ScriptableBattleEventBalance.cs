
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableBattleEventBalance")]
public class ScriptableBattleEventBalance : ScriptableObject
{
    [SerializeField]
    private BattleEventDefine type;
    public BattleEventDefine Type => type;

    public void SetType(BattleEventDefine newType)
    {
        type = newType;
    }

    [SerializeField]
    private float[] duration;

    [SerializeField]
    private float[] value;

    [SerializeField]
    private float[] applyIntervalSeconds;

    public float GetDuration(int level)
    {
        if (duration == null || duration.Length <= level)
            return 0f;

        return duration[level];
    }

    public float GetValue(int level)
    {
        if (value == null || value.Length <= level)
            return 0f;

        return value[level];
    }

    public float GetApplyIntervalSeconds(int level)
    {
        if (applyIntervalSeconds == null || applyIntervalSeconds.Length <= level)
            return 0f;

        return applyIntervalSeconds[level];
    }
}
