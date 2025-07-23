using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class BalanceCopyEditorWindow : EditorWindow
{
    private ScriptableObject sourceBalance;
    private ScriptableObject targetBalance;
    private Vector2 searchScrollPos;
    private string searchQuery = "";
    private Type balanceType;
    private Type defineType;

    public static void ShowWindow(ScriptableObject balance, Type balanceType, Type defineType)
    {
        BalanceCopyEditorWindow window = GetWindow<BalanceCopyEditorWindow>("Balance Copy");
        window.sourceBalance = balance;
        window.balanceType = balanceType;
        window.defineType = defineType;
        window.InitializeWindow(300, 400);
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("밸런스 데이터 복사", EditorStyles.boldLabel);
        EditorGUILayout.Space(10);

        if (sourceBalance == null)
        {
            EditorGUILayout.HelpBox("복사할 원본 밸런스 데이터가 선택되지 않았습니다.", MessageType.Error);
            return;
        }

        EditorGUILayout.LabelField($"원본: {sourceBalance.name} ({sourceBalance.GetType().Name})", EditorStyles.boldLabel);
        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField("복사 대상 밸런스 데이터 선택", EditorStyles.boldLabel);

        // 검색창
        searchQuery = EditorGUILayout.TextField("Search", searchQuery);

        // 검색어에 따라 필터링된 목록 표시
        if (!string.IsNullOrEmpty(searchQuery))
        {
            string[] allAssetGuids = AssetDatabase.FindAssets($"t:{balanceType.Name}", new string[] { PathDefine.PATH_ABILITY_BALANCE_FOLDER, PathDefine.PATH_BATTLE_EVENT_BALANCE_FOLDER });
            List<ScriptableObject> filteredBalances = new List<ScriptableObject>();

            foreach (string guid in allAssetGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                ScriptableObject balance = AssetDatabase.LoadAssetAtPath(path, balanceType) as ScriptableObject;
                if (balance != null && balance.name.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    filteredBalances.Add(balance);
                }
            }

            if (filteredBalances.Any())
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                searchScrollPos = EditorGUILayout.BeginScrollView(searchScrollPos, GUILayout.Height(100));

                GUIStyle searchResultStyle = new GUIStyle(GUI.skin.button)
                {
                    alignment = TextAnchor.MiddleLeft,
                    padding = new RectOffset(10, 4, 4, 4)
                };

                foreach (ScriptableObject balance in filteredBalances)
                {
                    if (GUILayout.Button(balance.name, searchResultStyle))
                    {
                        targetBalance = balance;
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
            // 검색어가 없을 때는 전체 목록 표시 (선택된 Define에 따라 필터링)
            string[] allAssetGuids = AssetDatabase.FindAssets($"t:{balanceType.Name}", new string[] { PathDefine.PATH_ABILITY_BALANCE_FOLDER, PathDefine.PATH_BATTLE_EVENT_BALANCE_FOLDER });
            List<ScriptableObject> allBalances = new List<ScriptableObject>();

            foreach (string guid in allAssetGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                ScriptableObject balance = AssetDatabase.LoadAssetAtPath(path, balanceType) as ScriptableObject;
                if (balance != null)
                {
                    allBalances.Add(balance);
                }
            }

            if (allBalances.Any())
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                searchScrollPos = EditorGUILayout.BeginScrollView(searchScrollPos, GUILayout.Height(200)); // 높이 조절

                foreach (ScriptableObject balance in allBalances)
                {
                    if (GUILayout.Button(balance.name))
                    {
                        targetBalance = balance;
                    }
                }
                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndVertical();
            }
            else
            {
                EditorGUILayout.HelpBox("선택할 밸런스 데이터가 없습니다.", MessageType.Info);
            }
        }

        EditorGUILayout.Space(10);

        if (targetBalance != null)
        {
            EditorGUILayout.LabelField($"선택된 대상: {targetBalance.name} ({targetBalance.GetType().Name})", EditorStyles.boldLabel);
            EditorGUILayout.Space(10);

            if (GUILayout.Button("확인 (값 복사)", GUILayout.Height(40)))
            {
                if (EditorUtility.DisplayDialog("값 복사 확인",
                    $"'{targetBalance.name}'의 값을 '{sourceBalance.name}'으로 복사하시겠습니까? 이 작업은 되돌릴 수 없습니다.",
                    "복사", "취소"))
                {
                    CopyScriptableObjectValues(sourceBalance, targetBalance);
                    EditorUtility.SetDirty(targetBalance);
                    AssetDatabase.SaveAssets();
                    EditorUtility.DisplayDialog("복사 완료", "값이 성공적으로 복사되었습니다.", "확인");
                    this.Close();
                }
            }
        }
    }

    private void CopyScriptableObjectValues(ScriptableObject source, ScriptableObject target)
    {
        if (source == null || target == null || source.GetType() != target.GetType())
        {
            Debug.LogError("원본과 대상 ScriptableObject의 타입이 다르거나 유효하지 않습니다.");
            return;
        }

        if (source is ScriptableBattleEventBalance beSource && target is ScriptableBattleEventBalance beTarget)
        {
            beSource.DeepCopy(beTarget); 
        }
        else if (source is ScriptableAbilityBalance aSource && target is ScriptableAbilityBalance aTarget)
        {
            aSource.DeepCopy(aTarget);
        }
    }
}
