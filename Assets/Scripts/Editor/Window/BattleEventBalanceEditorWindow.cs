using UnityEditor;
using UnityEngine;
using System.IO;

public class BattleEventBalanceEditorWindow : EditorWindow
{
    private BattleEventDefine selectedDefine;
    private ScriptableBattleEventBalance currentBalance;
    private string assetPath = PathDefine.PATH_BATTLE_EVENT_BALANCE_FOLDER;
    private Vector2 scrollPos;

    [MenuItem("Tools/Battle Event Balance/Editor")]
    public static void ShowWindow()
    {
        GetWindow<BattleEventBalanceEditorWindow>("Battle Event Balance Editor");
    }

    private int selectedTab = 0;

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Battle Event Balance Editor", EditorStyles.boldLabel);

        selectedTab = GUILayout.Toolbar(selectedTab, new string[] { "편집", "목록" });

        switch (selectedTab)
        {
            case 0:
                DrawEditTab();
                break;

            case 1:
                DrawListTab();
                break;
        }
    }

    private void DrawEditTab()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("편집", EditorStyles.boldLabel);

        selectedDefine = (BattleEventDefine)EditorGUILayout.EnumPopup("Select", selectedDefine);

        if (currentBalance != null)
        {
            if (selectedDefine != currentBalance.Type)
            {
                if (selectedDefine == BattleEventDefine.None)
                {
                    currentBalance = null;
                    return;
                }

                LoadAssetForEdit();

                if (currentBalance == null)
                    return;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("편집 중: " + currentBalance.name, EditorStyles.boldLabel);

            SerializedObject serializedObject = new SerializedObject(currentBalance);
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("duration"), new GUIContent("지속 시간"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("value"), new GUIContent("값"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("applyIntervalSeconds"), new GUIContent("적용 간격"));

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
                EditorUtility.SetDirty(currentBalance);

            if (GUILayout.Button("저장"))
                AssetDatabase.SaveAssets();
        }
        else
        {
            if (GUILayout.Button("Open"))
                LoadAssetForEdit();
        }
    }

    private void LoadAssetForEdit()
    {
        if (selectedDefine == BattleEventDefine.None)
        {
            EditorUtility.DisplayDialog("오류", "BattleEventDefine을 선택하세요.", "확인");
            return;
        }

        string fileName = selectedDefine.ToString() + ".asset";
        string fullPath = Path.Combine(assetPath, fileName);

        if (File.Exists(fullPath))
        {
            currentBalance = AssetDatabase.LoadAssetAtPath<ScriptableBattleEventBalance>(fullPath);
        }
        else
        {
            EditorUtility.DisplayDialog("알림", "해당 타입의 Balance가 존재하지 않습니다.\n갱신해주세요.", "확인");
            currentBalance = null;
        }
    }

    private void DrawListTab()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("목록", EditorStyles.boldLabel);

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        string[] guids = AssetDatabase.FindAssets("t:ScriptableBattleEventBalance", new string[] { assetPath });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ScriptableBattleEventBalance balance = AssetDatabase.LoadAssetAtPath<ScriptableBattleEventBalance>(path);

            if (balance != null)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(balance.name);
                if (GUILayout.Button("Edit", GUILayout.Width(60)))
                {
                    selectedDefine = balance.Type;
                    currentBalance = balance;
                    selectedTab = 0;
                    GUI.changed = true;
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        EditorGUILayout.EndScrollView();
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
