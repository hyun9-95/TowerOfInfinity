using UnityEditor;
using UnityEngine;
using System.IO;

public class BattleEventBalanceEditorWindow : BalanceEditorWindowBase<BattleEventBalanceEditorWindow, ScriptableBattleEventBalance, BattleEventDefine>
{
    protected override string EditorTitle => "Battle Event Balance Editor";
    protected override string[] TabTitles => new string[] { "편집", "목록" };
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
        EditorGUILayout.PropertyField(serializedObject.FindProperty("duration"), new GUIContent("지속 시간"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("value"), new GUIContent("값"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("applyIntervalSeconds"), new GUIContent("적용 간격"));
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
}


public static class BattleEventEditorUtil
{
    private static string assetPath = PathDefine.PATH_BATTLE_EVENT_BALANCE_FOLDER;

    [MenuItem("Tools/Battle Event Balance/Refresh All")]
    public static void RefreshAll()
    {
        System.Array enumValues = System.Enum.GetValues(typeof(BattleEventDefine));
        System.Collections.Generic.List<string> createdAssets = new System.Collections.Generic.List<string>();

        foreach (BattleEventDefine define in enumValues)
        {
            if (define == BattleEventDefine.None)
                continue;

            string fileName = define.ToString() + ".asset";
            string fullPath = Path.Combine(assetPath, fileName);

            if (!File.Exists(fullPath))
            {
                if (!Directory.Exists(assetPath))
                {
                    Directory.CreateDirectory(assetPath);
                }

                ScriptableBattleEventBalance newBalance = ScriptableObject.CreateInstance<ScriptableBattleEventBalance>();
                newBalance.SetType(define);
                AssetDatabase.CreateAsset(newBalance, fullPath);
                createdAssets.Add(fileName);
            }
        }

        if (createdAssets.Count > 0)
        {
            AssetDatabase.SaveAssets();
            string message = "새로운 BattleEventBalance 에셋 생성:\n" + string.Join("\n", createdAssets);
            Logger.Success(message);
        }

        // 기존 에셋 중 BattleEventDefine에 없는 에셋 제거
        string[] existingAssets = Directory.GetFiles(assetPath, "*.asset");
        System.Collections.Generic.List<string> deletedAssets = new System.Collections.Generic.List<string>();

        foreach (string asset in existingAssets)
        {
            string assetName = Path.GetFileNameWithoutExtension(asset);
            bool foundInEnum = false;
            foreach (BattleEventDefine define in enumValues)
            {
                if (define.ToString() == assetName)
                {
                    foundInEnum = true;
                    break;
                }
            }

            if (!foundInEnum)
            {
                AssetDatabase.DeleteAsset(asset);
                deletedAssets.Add(assetName);
            }
        }

        if (deletedAssets.Count > 0)
        {
            AssetDatabase.SaveAssets();
            string message = "다음 BattleEventBalance 에셋이 제거됨:\n" + string.Join("\n", deletedAssets);
            EditorUtility.DisplayDialog("제거 완료", message, "확인");
        }

        if (createdAssets.Count == 0 && deletedAssets.Count == 0)
            Logger.Log("ScriptableBattleEventBalance - 변경 사항이 없음.");
    }
}

