using UnityEditor;
using UnityEngine;
using System.IO;

public class AbilityBalanceEditorWindow : EditorWindow
{
    private AbilityDefine selectedDefine;
    private ScriptableAbilityBalance currentBalance;
    private string assetPath = PathDefine.PATH_ABILITY_BALANCE_FOLDER;
    private Vector2 scrollPos;

    [MenuItem("Tools/Ability Balance/Editor")]
    public static void ShowWindow()
    {
        GetWindow<AbilityBalanceEditorWindow>("Ability Balance Editor");
    }

    private int selectedTab = 0;

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Ability Balance Editor", EditorStyles.boldLabel);

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

        selectedDefine = (AbilityDefine)EditorGUILayout.EnumPopup("Select", selectedDefine);

        if (currentBalance != null)
        {
            if (selectedDefine != currentBalance.Type)
            {
                if (selectedDefine == AbilityDefine.None)
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

            // AbilityBalance에 맞는 필드들로 변경
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Speed"), new GUIContent("속도"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Range"), new GUIContent("범위"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Duration"), new GUIContent("지속 시간"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Scale"), new GUIContent("스케일"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("TargetCount"), new GUIContent("타겟 수"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("CoolTime"), new GUIContent("쿨타임"));


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
        if (selectedDefine == AbilityDefine.None)
        {
            EditorUtility.DisplayDialog("오류", "AbilityDefine을 선택하세요.", "확인");
            return;
        }

        string fileName = selectedDefine.ToString() + ".asset";
        string fullPath = Path.Combine(assetPath, fileName);

        if (File.Exists(fullPath))
        {
            currentBalance = AssetDatabase.LoadAssetAtPath<ScriptableAbilityBalance>(fullPath);
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

        string[] guids = AssetDatabase.FindAssets("t:ScriptableAbilityBalance", new string[] { assetPath });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ScriptableAbilityBalance balance = AssetDatabase.LoadAssetAtPath<ScriptableAbilityBalance>(path);

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