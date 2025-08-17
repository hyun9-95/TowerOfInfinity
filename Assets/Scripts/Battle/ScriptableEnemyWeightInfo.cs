using AYellowpaper.SerializedCollections;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Enemy Weight Info", fileName = "ScriptableEnemyWeightInfo")]
public class ScriptableEnemyWeightInfo : ScriptableObject
{
    [Header("Enemy Spawn Weights")]
    [SerializeField] 
    private SerializedDictionary<CharacterDefine, WeightInfo> enemyWeights = new();

    public float GetCurrentWeight(CharacterDefine characterDefine, int currentWave)
    {
        int endWave = IntDefine.MAX_DUNGEON_WAVE_COUNT;
        float waveProgress = currentWave / (float)endWave;

        if (enemyWeights.TryGetValue(characterDefine, out var weightInfo))
            return Mathf.Lerp(weightInfo.StartWeight, weightInfo.EndWeight, waveProgress);

        return 0;
    }

    public SerializedDictionary<CharacterDefine, WeightInfo> EnemyWeights => enemyWeights;
}

[System.Serializable]
public struct WeightInfo
{
    [Header("Weight Range")]
    [Tooltip("시작 웨이브에서의 가중치")]
    public float StartWeight;
    
    [Tooltip("마지막 웨이브에서의 가중치")]
    public float EndWeight;

    public WeightInfo(float startWeight, float endWeight)
    {
        StartWeight = startWeight;
        EndWeight = endWeight;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(WeightInfo))]
public class WeightInfoPropertyDrawer : PropertyDrawer
{
    private const float LABEL_WIDTH = 80f;
    private const float FIELD_SPACING = 5f;
    private const float LINE_HEIGHT = 18f;
    private const float PADDING = 2f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // 배경 박스 그리기
        Rect boxRect = new Rect(position.x, position.y, position.width, GetPropertyHeight(property, label));
        GUI.Box(boxRect, "", EditorStyles.helpBox);

        // 내부 여백 적용
        position.x += PADDING;
        position.y += PADDING;
        position.width -= PADDING * 2;

        // Start Weight 필드
        Rect startRect = new Rect(position.x, position.y, position.width, LINE_HEIGHT);
        SerializedProperty startWeightProp = property.FindPropertyRelative("StartWeight");
        
        EditorGUI.BeginChangeCheck();
        float startWeight = EditorGUI.FloatField(startRect, "Start Weight", startWeightProp.floatValue);
        if (EditorGUI.EndChangeCheck())
        {
            startWeightProp.floatValue = Mathf.Max(0f, startWeight);
        }

        // End Weight 필드
        Rect endRect = new Rect(position.x, position.y + LINE_HEIGHT + FIELD_SPACING, position.width, LINE_HEIGHT);
        SerializedProperty endWeightProp = property.FindPropertyRelative("EndWeight");
        
        EditorGUI.BeginChangeCheck();
        float endWeight = EditorGUI.FloatField(endRect, "End Weight", endWeightProp.floatValue);
        if (EditorGUI.EndChangeCheck())
        {
            endWeightProp.floatValue = Mathf.Max(0f, endWeight);
        }

        // 프리뷰 바 그리기
        Rect previewRect = new Rect(position.x, position.y + (LINE_HEIGHT + FIELD_SPACING) * 2, position.width, LINE_HEIGHT);
        DrawWeightPreview(previewRect, startWeightProp.floatValue, endWeightProp.floatValue);

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return (LINE_HEIGHT + FIELD_SPACING) * 3 + PADDING * 2;
    }

    private void DrawWeightPreview(Rect rect, float startWeight, float endWeight)
    {
        // 라벨
        Rect labelRect = new Rect(rect.x, rect.y, LABEL_WIDTH, rect.height);
        EditorGUI.LabelField(labelRect, "Preview:", EditorStyles.miniLabel);

        // 프리뷰 바 영역
        Rect barRect = new Rect(rect.x + LABEL_WIDTH + 5f, rect.y + 2f, rect.width - LABEL_WIDTH - 10f, rect.height - 4f);
        
        // 배경
        EditorGUI.DrawRect(barRect, new Color(0.3f, 0.3f, 0.3f, 0.5f));

        // 그라디언트 효과
        if (startWeight > 0f || endWeight > 0f)
        {
            float maxWeight = Mathf.Max(startWeight, endWeight, 1f);
            
            // 여러 세그먼트로 나누어 그라디언트 효과 구현
            int segments = 20;
            float segmentWidth = barRect.width / segments;
            
            for (int i = 0; i < segments; i++)
            {
                float progress = i / (float)(segments - 1);
                float currentWeight = Mathf.Lerp(startWeight, endWeight, progress);
                float normalizedHeight = currentWeight / maxWeight;
                
                Rect segmentRect = new Rect(
                    barRect.x + i * segmentWidth,
                    barRect.y + barRect.height * (1f - normalizedHeight),
                    segmentWidth,
                    barRect.height * normalizedHeight
                );

                // 색상 계산 (파란색에서 빨간색으로)
                Color segmentColor = Color.Lerp(Color.blue, Color.red, normalizedHeight);
                segmentColor.a = 0.8f;
                EditorGUI.DrawRect(segmentRect, segmentColor);
            }
        }

        // 수치 표시
        Rect valueRect = new Rect(barRect.x, barRect.y - 15f, barRect.width, 12f);
        string valueText = $"{startWeight:F1} → {endWeight:F1}";
        EditorGUI.LabelField(valueRect, valueText, EditorStyles.centeredGreyMiniLabel);
    }
}
#endif