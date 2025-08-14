#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class AppBuildWindow : EditorWindow
{
    private string buildPath = "";
    private bool fullBuild = false;
    private bool cheat = false;
    private bool debugLog = false;
    private string bundleVersion = "0.0.0";

    [MenuItem("Tools/Build/App Build Window")]
    public static void OpenAppBuildWindow()
    {
        AppBuildWindow window = GetWindow<AppBuildWindow>("App Build");
        window.minSize = new Vector2(400f, 200f);
        window.Show();
    }

    private void OnEnable()
    {
        LoadSettings();
    }

    private void OnDisable()
    {
        SaveSettings();
    }

    private void OnGUI()
    {
        GUILayout.Space(20f);

        EditorGUILayout.LabelField("App Build Configuration", EditorStyles.boldLabel);
        
        GUILayout.Space(10f);

        DrawBuildPathSection();
        
        GUILayout.Space(10f);

        DrawFullBuildToggle();
        
        GUILayout.Space(20f);

        DrawBuildButton();
    }

    private void DrawBuildPathSection()
    {
        EditorGUILayout.LabelField("Build Path:");
        
        EditorGUILayout.BeginHorizontal();
        buildPath = EditorGUILayout.TextField(buildPath);
        
        if (GUILayout.Button("Browse", GUILayout.Width(70f)))
        {
            string selectedPath = EditorUtility.OpenFolderPanel("Select Build Path", buildPath, "");
            if (!string.IsNullOrEmpty(selectedPath))
            {
                buildPath = selectedPath;
            }
        }
        EditorGUILayout.EndHorizontal();

        bundleVersion = EditorGUILayout.TextField("Bundle Version", bundleVersion);
    }

    private void DrawFullBuildToggle()
    {
        fullBuild = EditorGUILayout.Toggle("Full Build", fullBuild);
        cheat = EditorGUILayout.Toggle("Cheat", cheat);
        debugLog = EditorGUILayout.Toggle("Debug Log", debugLog);
    }

    private void DrawBuildButton()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Build", GUILayout.Width(150f), GUILayout.Height(40f)))
        {
#if UNITY_STANDALONE
            OnBuild(NamedBuildTarget.Standalone, BuildTarget.StandaloneWindows64);
#elif UNITY_ANDROID
            OnBuild(NamedBuildTarget.Android, BuildTarget.Android);
#endif
        }

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
    }

    private void OnBuild(NamedBuildTarget namedBuildTarget, BuildTarget buildTarget)
    {
        if (string.IsNullOrEmpty(buildPath))
        {
            EditorUtility.DisplayDialog("Error", "Buildpath is empty", "OK");
            return;
        }

        string originDefineSymbols = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget);
        PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, MakeDefineSymbols());
        PlayerSettings.bundleVersion = bundleVersion.ToString();

        AssetDatabase.Refresh();

        try
        {
            if (fullBuild)
            {
                // 어드레서블 빌드 후 빌드
                AddressableAssetSettings.BuildPlayerContent(out AddressablesPlayerBuildResult result);

                bool success = string.IsNullOrEmpty(result.Error);

                if (success)
                {
                    AppBuild(buildTarget);
                }
                else
                {
                    Logger.Error($"Full build faild => Bulild Addressable Failed!");
                    Logger.Error(result.Error);
                }
            }
            else
            {
                AppBuild(buildTarget);
            } 
        }
        catch (System.Exception e)
        {
            Logger.Exception("Build failed", e);
            EditorUtility.DisplayDialog("Build Failed", $"Exception : {e.Message}", "OK");
        }
        finally
        {
            PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.Standalone, originDefineSymbols);
        }
    }

    private void AppBuild(BuildTarget buildTarget)
    {
        var extension = buildTarget == BuildTarget.StandaloneWindows64 ? ".exe" : ".apk";
        var finalBuildPath = $"{buildPath}/{buildTarget}/App/TOI_{bundleVersion}{extension}";
       
        BuildReport report = BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, finalBuildPath, buildTarget, BuildOptions.None);

        if (report.summary.result == BuildResult.Succeeded)
        {
            Logger.Success($"Build Success => {buildPath}");
            EditorUtility.DisplayDialog("Build Success", $"Build Complete!\nPath: {buildPath}", "OK");
        }
        else
        {
            Logger.Error($"Build failed");
            EditorUtility.DisplayDialog("Build Failed", $"BBuild failed =>{report.summary.result}", "OK");
        }
    }
    
    private string MakeDefineSymbols()
    {
        var sb = GlobalStringBuilder.Get();

        if (cheat)
            sb.Append("CHEAT;");

        if (debugLog)
            sb.Append("DEBUG_LOG;");

        if (sb.Length > 0)
            sb.Length--;

        return sb.ToString();
    }

    private void LoadSettings()
    {
        buildPath = EditorPrefs.GetString("AppBuildWindow_BuildPath", Application.dataPath + "/../Builds");
        fullBuild = EditorPrefs.GetBool("AppBuildWindow_FullBuild", false);
        cheat = EditorPrefs.GetBool("AppBuildWindow_Cheat", false);
        bundleVersion = EditorPrefs.GetString("AppBuildWindow_BundleVersion", "0.0.0");
    }

    private void SaveSettings()
    {
        EditorPrefs.SetString("AppBuildWindow_BuildPath", buildPath);
        EditorPrefs.SetBool("AppBuildWindow_FullBuild", fullBuild);
    }
}
#endif