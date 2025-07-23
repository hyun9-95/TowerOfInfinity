using UnityEditor;
using UnityEngine;

public class AbilityBalanceEditorWindow : BalanceEditorWindowBase<AbilityBalanceEditorWindow, ScriptableAbilityBalance, AbilityDefine>
{
    protected override string EditorTitle => "Ability Balance Editor";
    protected override string[] TabTitles => new string[] { "편집", "목록" };
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
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Speed"), new GUIContent("속도"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Range"), new GUIContent("범위"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Duration"), new GUIContent("지속 시간"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Scale"), new GUIContent("스케일"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("TargetCount"), new GUIContent("타겟 수"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("CoolTime"), new GUIContent("쿨타임"));
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
}
