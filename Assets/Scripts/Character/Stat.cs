using UnityEngine;
[System.Serializable]
public struct Stat
{
    public StatType StatType => statType;
    public float Value => value;

    [SerializeField]
    private StatType statType;

    [SerializeField]
    private float value;

    public Stat(StatType type, float statValue)
    {
        statType = type;
        value = statValue;
    }
}
