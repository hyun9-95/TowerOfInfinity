using UnityEditor;
using UnityEngine;

public class AbilityBalanceEditorWindow : BalanceEditorWindowBase<AbilityBalanceEditorWindow, ScriptableAbilityBalance, AbilityDefine>
{
    protected override string EditorTitle => "Ability Balance Editor";
    protected override string[] TabTitles => new string[] { "편집", "관리" };
    protected override string AssetFilter => "t:ScriptableAbilityBalance";

    [MenuItem("Tools/Ability Balance/Editor")]
    public static void ShowWindow()
    {
        ShowWindow<AbilityBalanceEditorWindow>("Ability Balance Editor");
    }

    private void OnEnable()
    {
        assetPath = PathDefine.PATH_ABILITY_BALANCE_FOLDER;
    }

    protected override void DrawBalanceSettings(SerializedObject serializedObject)
    {
        DrawArrayPropertyField(serializedObject, "Speed", "속도");
        DrawArrayPropertyField(serializedObject, "Range", "범위");
        DrawArrayPropertyField(serializedObject, "Duration", "지속 시간");
        DrawArrayPropertyField(serializedObject, "Scale", "스케일");
        DrawArrayPropertyField(serializedObject, "TargetCount", "타겟 수");
        DrawArrayPropertyField(serializedObject, "CoolTime", "쿨타임");
    }

    private void DrawArrayPropertyField(SerializedObject serializedObject, string propertyName, string label)
    {
        SerializedProperty property = serializedObject.FindProperty(propertyName);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.PropertyField(property, new GUIContent(label));

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Preset"))
        {
            float initialValue = 0f;
            if (property.isArray && property.arraySize > 0)
            {
                initialValue = property.GetArrayElementAtIndex(0).floatValue;
            }
            ArrayValuePresetEditorWindow.ShowWindow(property, IntDefine.MAX_ABILITY_LEVEL, initialValue);
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(5);
    }

    protected override AbilityDefine GetDefineFromBalance(ScriptableAbilityBalance balance)
    {
        return balance.Type;
    }

    protected override bool IsDefineSelected(AbilityDefine define, ScriptableAbilityBalance balance)
    {
        return define == balance.Type;
    }

    protected override bool IsNone(AbilityDefine define)
    {
        return define == AbilityDefine.None;
    }

    protected override void SetBalanceType(ScriptableAbilityBalance balance, AbilityDefine define)
    {
        balance.SetType(define);
    }

    protected override void ResetBalanceValues(ScriptableAbilityBalance balance)
    {
        balance.ResetBalanceValues();
    }
}
