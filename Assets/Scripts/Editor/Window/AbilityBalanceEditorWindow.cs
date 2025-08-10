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
        DrawArrayPropertyField(serializedObject, "CoolTime", "쿨타임");
        DrawArrayPropertyField(serializedObject, "Duration", "지속 시간");
        DrawArrayPropertyField(serializedObject, "HitCount", "Hit 횟수");
        DrawArrayPropertyField(serializedObject, "HitForce", "Hit 시 미는 힘");
        DrawArrayPropertyField(serializedObject, "SpawnCount", "Unit 생성 개수");
        DrawArrayPropertyField(serializedObject, "Range", "범위");
        DrawArrayPropertyField(serializedObject, "Speed", "속도");
        DrawArrayPropertyField(serializedObject, "Scale", "유닛 스케일");
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
