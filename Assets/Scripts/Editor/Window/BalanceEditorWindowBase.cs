using UnityEditor;
using UnityEngine;
using System.IO;
using System;

public abstract class BalanceEditorWindowBase<T, U, V> : EditorWindow
    where T : EditorWindow
    where U : ScriptableObject
    where V : Enum
{
    protected V selectedDefine;
    protected U currentBalance;
    protected string assetPath;
    private Vector2 scrollPos;
    private int selectedTab = 0;

    protected abstract string EditorTitle { get; }
    protected abstract string[] TabTitles { get; }
    protected abstract string AssetFilter { get; }


    protected static void ShowWindow<W>(string title) where W : EditorWindow
    {
        GetWindow<W>(title);
    }

    protected virtual void OnGUI()
    {
        EditorGUILayout.LabelField(EditorTitle, EditorStyles.boldLabel);
        selectedTab = GUILayout.Toolbar(selectedTab, TabTitles);

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

    protected virtual void DrawEditTab()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("편집", EditorStyles.boldLabel);

        selectedDefine = (V)(object)EditorGUILayout.EnumPopup("Select", selectedDefine);

        if (currentBalance != null)
        {
            if (!IsDefineSelected(selectedDefine, currentBalance))
            {
                if (IsNone(selectedDefine))
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

            DrawBalanceSettings(serializedObject);

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
                EditorUtility.SetDirty(currentBalance);

            if (GUILayout.Button("저장"))
                AssetDatabase.SaveAssets();
        }
        else
        {
            if (!IsNone(selectedDefine))
                LoadAssetForEdit();
        }
    }

    protected abstract void DrawBalanceSettings(SerializedObject serializedObject);
    protected abstract bool IsDefineSelected(V define, U balance);
    protected abstract bool IsNone(V define);
    protected abstract void SetBalanceType(U balance, V define);

    protected void LoadAssetForEdit()
    {
        if (IsNone(selectedDefine))
        {
            EditorUtility.DisplayDialog("오류", "Define을 선택하세요.", "확인");
            return;
        }

        string fileName = selectedDefine.ToString() + ".asset";
        string fullPath = Path.Combine(assetPath, fileName);

        if (File.Exists(fullPath))
        {
            currentBalance = AssetDatabase.LoadAssetAtPath<U>(fullPath);
        }
        else
        {
            EditorUtility.DisplayDialog("알림", "해당 타입의 Balance가 존재하지 않습니다.\n갱신해주세요.", "확인");
            currentBalance = null;
            selectedDefine = default;
        }
    }

    protected void DrawListTab()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("목록", EditorStyles.boldLabel);

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        string[] guids = AssetDatabase.FindAssets(AssetFilter, new string[] { assetPath });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            U balance = AssetDatabase.LoadAssetAtPath<U>(path);

            if (balance != null)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(balance.name);
                if (GUILayout.Button("Edit", GUILayout.Width(60)))
                {
                    selectedDefine = GetDefineFromBalance(balance);
                    currentBalance = balance;
                    selectedTab = 0;
                    GUI.changed = true;
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        EditorGUILayout.EndScrollView();
    }

    protected abstract V GetDefineFromBalance(U balance);
}