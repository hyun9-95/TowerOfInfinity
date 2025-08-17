using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableCharacterStat", menuName = "ScriptableObject/Character/Character Stat")]
public class ScriptableCharacterStat : ScriptableObject
{
    [SerializeField]
    private Stat[] stats;

    public Stat GetStat(StatType statType)
    {
        if (stats == null)
            return default;

        // None이 있으므로 1빼줌
        int index = (int)statType;

        if (index >= stats.Length)
            return default;

        return stats[index];
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        int statLength = System.Enum.GetValues(typeof(StatType)).Length - 1;

        if (stats == null || stats.Length == 0)
        {
            stats = new Stat[statLength];

            for (int i = 0; i < statLength; i++)
                stats[i] = new Stat((StatType)i, 0);

            Logger.Log("Stat Initialized");
        }
    }
#endif
}
