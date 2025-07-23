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
        DrawArrayPropertyField(serializedObject, "value", "값");
        DrawArrayPropertyField(serializedObject, "duration", "지속 시간");
        DrawArrayPropertyField(serializedObject, "applyIntervalSeconds", "적용 간격");
    }

    protected override BattleEventDefine GetDefineFromBalance(ScriptableBattleEventBalance balance)
    {
        return balance.Define;
    }

    protected override bool IsDefineSelected(BattleEventDefine define, ScriptableBattleEventBalance balance)
    {
        return define == balance.Define;
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

