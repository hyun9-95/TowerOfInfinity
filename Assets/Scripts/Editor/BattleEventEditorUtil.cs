using System.IO;
using UnityEditor;
using UnityEngine;

public static class BattleEventEditorUtil
{
    private static string assetPath = PathDefine.PATH_BATTLE_EVENT_BALANCE_FOLDER;

    [MenuItem("Tools/Balance/Refresh All Battle Events")]
    public static void RefreshAll()
    {
        RefreshAll(true);
    }

    public static void RefreshAll(bool allowDelete)
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
                newBalance.ResetBalanceValues();
                AssetDatabase.CreateAsset(newBalance, fullPath);
                createdAssets.Add(fileName);
            }
            else
            {
                // 파일이 이미 존재하는 경우
                ScriptableBattleEventBalance existingBalance = AssetDatabase.LoadAssetAtPath<ScriptableBattleEventBalance>(fullPath);
                if (existingBalance != null)
                {
                    bool isDirty = false;
                    
                    if (!existingBalance.Define.Equals(define))
                    {
                        existingBalance.SetType(define);
                        isDirty = true;
                        Logger.Log($"{fileName} 타입 갱신 {existingBalance.Define} => {define}");
                    }
                    
                    existingBalance.TruncateArraysToMaxLevel();
                    isDirty = true;
                    
                    if (isDirty)
                    {
                        EditorUtility.SetDirty(existingBalance);
                    }
                }
            }
        }

        if (createdAssets.Count > 0)
        {
            AssetDatabase.SaveAssets();
            string message = "새로운 BattleEventBalance 에셋 생성:\n" + string.Join("\n", createdAssets);
            Logger.Success(message);
        }

        System.Collections.Generic.List<string> deletedAssets = new System.Collections.Generic.List<string>();

        if (allowDelete)
        {
            // 기존 에셋 중 BattleEventDefine에 없는 에셋 제거
            string[] existingAssets = Directory.GetFiles(assetPath, "*.asset");

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
        }

        if (createdAssets.Count == 0 && deletedAssets.Count == 0)
            Logger.Log("ScriptableBattleEventBalance - 변경 사항이 없음.");
    }
}