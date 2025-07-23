using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;

public static class AbilityEditorUtil
{
    private static string assetPath = PathDefine.PATH_ABILITY_BALANCE_FOLDER;

    [MenuItem("Tools/Ability Balance/Refresh All")]
    public static void RefreshAll()
    {
        System.Array enumValues = System.Enum.GetValues(typeof(AbilityDefine));
        List<string> createdAssets = new List<string>();

        foreach (AbilityDefine define in enumValues)
        {
            if (define == AbilityDefine.None)
                continue;

            string fileName = define.ToString() + ".asset";
            string fullPath = Path.Combine(assetPath, fileName);

            if (!File.Exists(fullPath))
            {
                if (!Directory.Exists(assetPath))
                {
                    Directory.CreateDirectory(assetPath);
                }

                ScriptableAbilityBalance newBalance = ScriptableObject.CreateInstance<ScriptableAbilityBalance>();
                newBalance.SetType(define);
                AssetDatabase.CreateAsset(newBalance, fullPath);
                createdAssets.Add(fileName);
            }
        }

        if (createdAssets.Count > 0)
        {
            AssetDatabase.SaveAssets();
            string message = "새로운 AbilityBalance 에셋 생성:\n" + string.Join("\n", createdAssets);
            Logger.Success(message);
        }

        // 기존 에셋 중 AbilityDefine에 없는 에셋 제거
        string[] existingAssets = Directory.GetFiles(assetPath, "*.asset");
        List<string> deletedAssets = new List<string>();

        foreach (string asset in existingAssets)
        {
            string assetName = Path.GetFileNameWithoutExtension(asset);
            bool foundInEnum = false;
            foreach (AbilityDefine define in enumValues)
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
            string message = "다음 AbilityBalance 에셋이 제거됨:\n" + string.Join("\n", deletedAssets);
            EditorUtility.DisplayDialog("제거 완료", message, "확인");
        }

        if (createdAssets.Count == 0 && deletedAssets.Count == 0)
            Logger.Log("ScriptableAbilityBalance - 변경 사항이 없음.");
    }
}