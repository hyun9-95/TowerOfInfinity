using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;

public static class SOBalanceMonitor
{
    private enum RefreshType
    {
        AbilityDefine,
        BattleEventDefine
    }

    private static string[] defineFilePaths = new string[]
    {
        "Assets/Data/Scripts/Define/AbilityDefine.cs",
        "Assets/Data/Scripts/Define/BattleEventDefine.cs",
    };

    private static string[] refreshKeys = new string[]
    {
        "RefreshAbilityDefine",
        "RefreshBattleEventDefine"
    };

    public class DefineEnumAssetPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(
            string[] importedAssets, string[] deletedAssets,
            string[] movedAssets, string[] movedFromAssetPaths)
        {
            for (int i = 0; i < defineFilePaths.Length; i++)
            {
                if (importedAssets.Any(path => path.Replace("\\", "/").Equals(defineFilePaths[i])))
            {
                    Logger.Log($"{defineFilePaths[i]} 변경 감지 => 갱신 예정..");
                    EditorPrefs.SetBool(refreshKeys[i], true);
                }
            }
        }
    }

    [DidReloadScripts]
    private static void OnScriptsReloaded()
    {
        for (int i = 0; i< refreshKeys.Length; i++)
        {
            if (!EditorPrefs.GetBool(refreshKeys[i]))
                continue;

            EditorPrefs.SetBool(refreshKeys[i], false);
            RefreshEnumDefine((RefreshType)i);
        }
    }

    private static void RefreshEnumDefine(RefreshType type)
    {
        switch (type)
        {
            case RefreshType.AbilityDefine:
                AbilityEditorUtil.RefreshAll(false);
                break;

            case RefreshType.BattleEventDefine:
                BattleEventEditorUtil.RefreshAll(false);
                break;
        }
    }
}
