using UnityEditor;
using UnityEngine;
using System.IO;
using System;
using System.Linq;
using System.Collections.Generic;

public abstract class BalanceEditorWindowBase<T, U, V> : EditorWindow
    where T : EditorWindow
    where U : ScriptableObject
    where V : Enum
{
    protected V selectedDefine;
    protected U currentBalance;
    protected string assetPath;
    private Vector2 editScrollPos;
    private Vector2 listScrollPos;
    private Vector2 searchScrollPos;
    private int selectedTab = 0;
    private string searchQuery = "";
    private bool showResetOptions = false;

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
        EditorGUILayout.Space(10);
        selectedTab = GUILayout.Toolbar(selectedTab, TabTitles);

        switch (selectedTab)
        {
            case 0:
                DrawEditTab();
                break;
            case 1:
                DrawManageTab();
                break;
        }
    }

    protected virtual void DrawEditTab()
    {
        EditorGUILayout.Space();

        // 검색창
        searchQuery = EditorGUILayout.TextField("Search", searchQuery);

        // 검색어에 따라 필터링된 목록 표시
        if (!string.IsNullOrEmpty(searchQuery))
        {
            string[] allDefineNames = Enum.GetNames(typeof(V));
            List<string> filteredNames = allDefineNames
                .Where(name => name.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();

            if (filteredNames.Any())
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                searchScrollPos = EditorGUILayout.BeginScrollView(searchScrollPos, GUILayout.Height(100));

                GUIStyle searchResultStyle = new GUIStyle(GUI.skin.button)
                {
                    alignment = TextAnchor.MiddleLeft,
                    padding = new RectOffset(10, 4, 4, 4)
                };

                foreach (string name in filteredNames)
                {
                    if (GUILayout.Button(name, searchResultStyle))
                    {
                        selectedDefine = (V)Enum.Parse(typeof(V), name);
                        searchQuery = ""; // 선택 후 검색창 비우기
                        GUI.FocusControl(null); // 포커스 해제
                    }
                }
                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndVertical();
            }
            else
            {
                EditorGUILayout.HelpBox("검색 결과가 없습니다.", MessageType.Info);
            }
        }
        else
        {
            // 검색어가 없을 때는 EnumPopup 표시
            selectedDefine = (V)(object)EditorGUILayout.EnumPopup("Select", selectedDefine);
        }

        EditorGUILayout.Space(15);

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

            EditorGUILayout.LabelField("편집 중: " + currentBalance.name, EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            SerializedObject serializedObject = new SerializedObject(currentBalance);
            serializedObject.Update();

            editScrollPos = EditorGUILayout.BeginScrollView(editScrollPos);
            DrawBalanceSettings(serializedObject);
            EditorGUILayout.EndScrollView();

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
                EditorUtility.SetDirty(currentBalance);

            EditorGUILayout.Space(10);

            if (GUILayout.Button("저장", GUILayout.Height(30)))
                AssetDatabase.SaveAssets();
        }
        else
        {
            if (!IsNone(selectedDefine))
            {
                LoadAssetForEdit();
            }
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

    protected abstract void ResetBalanceValues(U balance);

    protected void DrawManageTab()
    {
        EditorGUILayout.Space();

        showResetOptions = EditorGUILayout.Foldout(showResetOptions, "Utils", true);

        if (showResetOptions)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.Space(5);
            if (GUILayout.Button("모두 초기화", GUILayout.Height(30)))
            {
                if (EditorUtility.DisplayDialog("경고", 
                    "모든 데이터의 배열 값을 초기화하시겠습니까? 이 작업은 되돌릴 수 없습니다.", 
                    "확인", "취소"))
                {
                    var resetGuids = AssetDatabase.FindAssets(AssetFilter, new string[] { assetPath });
                    foreach (string guid in resetGuids)
                    {
                        string path = AssetDatabase.GUIDToAssetPath(guid);
                        U balance = AssetDatabase.LoadAssetAtPath<U>(path);
                        if (balance != null)
                        {
                            ResetBalanceValues(balance);
                            EditorUtility.SetDirty(balance);
                        }
                    }
                    AssetDatabase.SaveAssets();
                    EditorUtility.DisplayDialog("완료", "모든 데이터가 초기화되었습니다.", "확인");
                }
            }
            EditorGUILayout.Space(5);
            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.Space(15);
        EditorGUILayout.LabelField("목록", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        listScrollPos = EditorGUILayout.BeginScrollView(listScrollPos);

        string[] guids = AssetDatabase.FindAssets(AssetFilter, new string[] { assetPath });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            U balance = AssetDatabase.LoadAssetAtPath<U>(path);

            if (balance != null)
            {
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                EditorGUILayout.LabelField(balance.name);
                if (GUILayout.Button("Edit", GUILayout.Width(80), GUILayout.Height(25)))
                {
                    selectedDefine = GetDefineFromBalance(balance);
                    currentBalance = balance;
                    selectedTab = 0;
                    GUI.changed = true;
                }
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(2);
            }
        }

        EditorGUILayout.EndScrollView();
    }

    protected void DrawArrayPropertyField(SerializedObject serializedObject, string propertyName, string label)
    {
        SerializedProperty property = serializedObject.FindProperty(propertyName);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        string currentValues = "";
        if (property.isArray)
        {
            for (int i = 0; i < property.arraySize; i++)
            {
                if (property.GetArrayElementAtIndex(i).propertyType == SerializedPropertyType.Float)
                {
                    currentValues += property.GetArrayElementAtIndex(i).floatValue.ToString("F2");
                }
                else if (property.GetArrayElementAtIndex(i).propertyType == SerializedPropertyType.Integer)
                {
                    currentValues += property.GetArrayElementAtIndex(i).intValue.ToString();
                }

                if (i < property.arraySize - 1)
                {
                    currentValues += " / ";
                }
            }
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(label, GUILayout.Width(EditorGUIUtility.labelWidth - 4)); // 라벨

        GUIStyle rightAlignedStyle = new GUIStyle(GUI.skin.label);
        rightAlignedStyle.alignment = TextAnchor.MiddleRight;
        EditorGUILayout.LabelField(currentValues, rightAlignedStyle);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.PropertyField(property, GUIContent.none); // 실제 PropertyField는 라벨 없이 그림

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Preset"))
        {
            float initialValue = 0f;
            if (property.isArray && property.arraySize > 0)
            {
                SerializedProperty element = property.GetArrayElementAtIndex(0);

                if (element.propertyType == SerializedPropertyType.Float)
                {
                    initialValue = element.floatValue;
                }
                else if (element.propertyType == SerializedPropertyType.Integer)
                {
                    initialValue = element.intValue; // float으로 변환돼도 무방
                }
            }
            ArrayValuePresetEditorWindow.ShowWindow(property, IntDefine.MAX_ABILITY_LEVEL, initialValue);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(5);
    }

    protected abstract V GetDefineFromBalance(U balance);
}