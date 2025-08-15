using UnityEngine;

[CreateAssetMenu(menuName = "Battle/Enemy Weight Info", fileName = "BattleEnemyWeightInfo")]
public class BattleEnemyWeightInfo : ScriptableObject
{
    [Header("Enemy Spawn Weights")]
    [SerializeField] private SerializableDictionary<CharacterDefine, float> enemyWeights = new SerializableDictionary<CharacterDefine, float>();

    public SerializableDictionary<CharacterDefine, float> EnemyWeights => enemyWeights;

    /// <summary>
    /// 특정 캐릭터의 가중치를 가져옵니다
    /// </summary>
    /// <param name="characterDefine">캐릭터 정의</param>
    /// <returns>가중치 (없으면 0)</returns>
    public float GetWeight(CharacterDefine characterDefine)
    {
        return enemyWeights.TryGetValue(characterDefine, out float weight) ? weight : 0f;
    }

    /// <summary>
    /// 특정 캐릭터의 가중치를 설정합니다
    /// </summary>
    /// <param name="characterDefine">캐릭터 정의</param>
    /// <param name="weight">가중치</param>
    public void SetWeight(CharacterDefine characterDefine, float weight)
    {
        enemyWeights[characterDefine] = weight;
    }

    /// <summary>
    /// 모든 가중치의 합을 계산합니다
    /// </summary>
    /// <returns>총 가중치</returns>
    public float GetTotalWeight()
    {
        float total = 0f;
        foreach (var kvp in enemyWeights)
        {
            total += kvp.Value;
        }
        return total;
    }
}