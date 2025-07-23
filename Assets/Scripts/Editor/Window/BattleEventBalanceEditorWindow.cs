using UnityEditor;
using UnityEngine;
using System.IO;

public class BattleEventBalanceEditorWindow : BalanceEditorWindowBase<BattleEventBalanceEditorWindow, ScriptableBattleEventBalance, BattleEventDefine>
{
    protected override string EditorTitle => "Battle Event Balance Editor";
    protected override string[] TabTitles => new string[] { "편집", "관리" };
    protected override string AssetFilter => "t:ScriptableBattleEventBalance";

    [MenuItem("Tools/Battle Event Balance/Editor")]
    public static void ShowWindow()
    {
        ShowWindow<BattleEventBalanceEditorWindow>("Battle Event Balance Editor");
    }

    private void OnEnable()
    {
        assetPath = PathDefine.PATH_BATTLE_EVENT_BALANCE_FOLDER;
    }

    protected override void DrawBalanceSettings(SerializedObject serializedObject)
    {
        DrawArrayPropertyField(serializedObject, "duration", "지속 시간");
        DrawArrayPropertyField(serializedObject, "value", "값");
        DrawArrayPropertyField(serializedObject, "applyIntervalSeconds", "적용 간격");
    }

    private void DrawArrayPropertyField(SerializedObject serializedObject, string propertyName, string label)
    {
        SerializedProperty property = serializedObject.FindProperty(propertyName);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.PropertyField(property, new GUIContent(label));

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("ValuePreset", GUILayout.Width(100)))
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

    protected override BattleEventDefine GetDefineFromBalance(ScriptableBattleEventBalance balance)
    {
        return balance.Type;
    }

    protected override bool IsDefineSelected(BattleEventDefine define, ScriptableBattleEventBalance balance)
    {
        return define == balance.Type;
    }

    protected override bool IsNone(BattleEventDefine define)
    {
        return define == BattleEventDefine.None;
    }

    protected override void SetBalanceType(ScriptableBattleEventBalance balance, BattleEventDefine define)
    {
        balance.SetType(define);
    }

    protected override void ResetBalanceValues(ScriptableBattleEventBalance balance)
    {
        balance.ResetBalanceValues();
    }
}

