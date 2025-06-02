#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public class BookmarkAsset : EditorWindow
{
    private enum Shortcuts
    {
        None,
        Ctrl_F3,
        Ctrl_F4,
    }

    [MenuItem("Tools/Util/BookmarkAsset/Open", false, 15)]
    public static void ShowWindow()
    {
        BookmarkAsset window = (BookmarkAsset)GetWindow(typeof(BookmarkAsset));
        window.titleContent.text = "BookmarkAsset";
        window.LoadPaths();
        window.LoadShortcuts();
    }

    [MenuItem("Tools/Util/BookmarkAsset/Play Root Scene %F1")]
    public static void PlayRootScene()
    {
        UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Resources/Scenes/RootScene.unity");
        EditorApplication.isPlaying = true;
    }


    [MenuItem("Tools/Util/BookmarkAsset/Open Custom Asset2 %F3", false, 15)]
    public static void OpenShortCut_Ctrl_F3()
    {
        string path = PlayerPrefs.GetString(PlayerPrefsKey + Shortcuts.Ctrl_F3);

        OpenShortcuts(Shortcuts.Ctrl_F3, path);
    }

    [MenuItem("Tools/Util/BookmarkAsset/Open Custom Asset3 %F4", false, 15)]
    public static void OpenShortCut_Ctrl_F4()
    {
        string path = PlayerPrefs.GetString(PlayerPrefsKey + Shortcuts.Ctrl_F4);

        OpenShortcuts(Shortcuts.Ctrl_F4, path);
    }

    private static void OpenShortcuts(Shortcuts shortcuts, string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError($"단축키로 지정된 에셋이 없습니다.. {shortcuts}");
            return;
        }

        Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(Object));

        if (obj == null)
        {
            Debug.Log($"에셋이 없습니다. {path}");
            return;
        }

        AssetDatabase.OpenAsset(obj);
    }

    #region Coding rule : Property
    #endregion Coding rule : Property

    #region Coding rule : Value

    private List<Object> assets = new List<Object>();
    private Vector2 scrollPosition;
    private const string PlayerPrefsKey = "BOOKMARK_ASSET_";
    private Dictionary<Shortcuts, string> shortcutsDic;
    #endregion Coding rule : Value

    #region Coding rule : Function
    private void OnGUI()
    {
        try
        {
            EditorGUILayout.LabelField("[Asset List]");

            EditorGUILayout.Space();

            if (shortcutsDic == null)
            {
                LoadPaths();
                LoadShortcuts();
            }

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < assets.Count; i++)
            {
                
                string labelField = AssetDatabase.GetAssetPath(assets[i]);

                string addressableResorucesPath = "Assets/IronJade/ResourcesAddressable/";
                string resorcesPath = "Assets/IronJade/Resources/";

                if (labelField.Contains(addressableResorucesPath))
                    labelField = labelField.Replace(addressableResorucesPath, "");

                if (labelField.Contains(resorcesPath))
                    labelField = labelField.Replace(resorcesPath, "");

                string[] splits = labelField.Split("/");

                sb.Clear();

                for (int s = 0; s < splits.Length; s++)
                {
                    if (s == splits.Length - 1)
                        continue;

                    sb.Append($"{splits[s]}/");
                }

                labelField = sb.ToString();
                
                GUIStyle grayLabelStyle = new GUIStyle(GUI.skin.label)
                {
                    normal = { textColor = Color.gray },
                    fontSize = 11
                };

                EditorGUILayout.LabelField($"{labelField}", grayLabelStyle);

                EditorGUILayout.BeginHorizontal();

                assets[i] = EditorGUILayout.ObjectField("", assets[i], typeof(Object), false);

                if (assets[i] != null)
                {
                    if (GUILayout.Button("Open", GUILayout.Width(60f)))
                        AssetDatabase.OpenAsset(assets[i]);

                    string path = AssetDatabase.GetAssetPath(assets[i]);
                    Shortcuts currentShortcut = GetCurrentShortcut(path);
                    Shortcuts newShortcut = (Shortcuts)EditorGUILayout.EnumPopup(currentShortcut, GUILayout.Width(70f));

                    if (newShortcut != currentShortcut)
                        UpdateShortcutAssignment(path, newShortcut);
                }

                if (GUILayout.Button($"Del", GetColorButtonStyle(new Color32(200, 100, 100, 255), 30, 18, 10)))
                {
                    string path = AssetDatabase.GetAssetPath(assets[i]);
                    Shortcuts currentShortcut = GetCurrentShortcut(path);

                    if (currentShortcut != Shortcuts.None)
                        UpdateShortcutAssignment(path, Shortcuts.None);

                    assets.RemoveAt(i);

                    // 에러 로그 방지
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndScrollView();
                    return;
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();
            }

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Add"))
                assets.Add(null);

            if (GUILayout.Button("Save"))
            {
                SavePaths();
                SaveShortcuts();
                Debug.Log("BookmarkAsset => Save.");
            }

            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Refresh"))
            {
                LoadPaths();
                LoadShortcuts();
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField($"* PlayerPrefsKey : {PlayerPrefsKey}");
            EditorGUILayout.LabelField($"* 단축키도 Save해야 적용됩니다.");

            EditorGUILayout.EndScrollView();
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }

    private void SavePaths()
    {
        List<string> pathList = assets.Where(x => x != null)
                                          .Select(x => AssetDatabase.GetAssetPath(x))
                                          .ToList();

        string serializedPathList = string.Join(";", pathList);

        PlayerPrefs.SetString(PlayerPrefsKey, serializedPathList);
        PlayerPrefs.Save();
    }

    private void LoadPaths()
    {
        assets.Clear();
        string serializedPathList = PlayerPrefs.GetString(PlayerPrefsKey, "");

        if (!string.IsNullOrEmpty(serializedPathList))
        {
            string[] paths = serializedPathList.Split(';');

            foreach (var path in paths)
            {
                Object loadedAsset = AssetDatabase.LoadAssetAtPath<Object>(path);

                if (loadedAsset == null)
                {
                    Debug.Log($"Asset is null!... path : {path}");
                    continue;
                }

                assets.Add(loadedAsset);
            }
        }
        else
        {
            //아무것도 없으면 하나만 추가.
            assets.Add(null);
        }
    }

    private void SaveShortcuts()
    {
        for (Shortcuts shortcuts = Shortcuts.Ctrl_F3; shortcuts <= Shortcuts.Ctrl_F4; shortcuts++)
        {
            if (!shortcutsDic.ContainsKey(shortcuts) || shortcutsDic[shortcuts] == null)
                return;

            PlayerPrefs.SetString(PlayerPrefsKey + shortcuts.ToString(), shortcutsDic[shortcuts]);
        }
    }

    private void LoadShortcuts()
    {
        shortcutsDic = new Dictionary<Shortcuts, string>();

        for (Shortcuts shortcuts = Shortcuts.Ctrl_F3; shortcuts <= Shortcuts.Ctrl_F4; shortcuts++)
        {
            string shortcutsPath = PlayerPrefs.GetString(PlayerPrefsKey + shortcuts.ToString());

            if (!string.IsNullOrEmpty(shortcutsPath))
                shortcutsDic.Add(shortcuts, shortcutsPath);
        }
    }

    private Shortcuts GetCurrentShortcut(string assetPath)
    {
        return shortcutsDic.FirstOrDefault(x => x.Value == assetPath).Key;
    }

    private void UpdateShortcutAssignment(string assetPath, Shortcuts newShortcut)
    {
        if (newShortcut == Shortcuts.None)
        {
            var deleteShortcuts = shortcutsDic.FirstOrDefault(x => x.Value == assetPath).Key;

            if (deleteShortcuts != default)
            {
                shortcutsDic[deleteShortcuts] = null;
                PlayerPrefs.SetString(PlayerPrefsKey + deleteShortcuts.ToString(), null);
            }

            return;
        }

        if (shortcutsDic.ContainsKey(newShortcut))
            shortcutsDic[newShortcut] = null;

        shortcutsDic[newShortcut] = assetPath;
    }

    private GUIStyle GetDefaultButtonStyle()
    {
        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fontStyle = FontStyle.Bold;
        buttonStyle.normal.textColor = Color.black;

        return buttonStyle;
    }

    private GUIStyle GetColorButtonStyle(Color32 color, int width, int height, int fontSize = 0)
    {
        GUIStyle buttonStyle = GetDefaultButtonStyle();
        buttonStyle.fixedWidth = width;
        buttonStyle.fixedHeight = height;

        if (fontSize > 0)
            buttonStyle.fontSize = fontSize;

        Texture2D colorTexture = new Texture2D(1, 1);
        colorTexture.SetPixel(0, 0, color);
        colorTexture.Apply();

        buttonStyle.normal.background = colorTexture;

        return buttonStyle;
    }
    #endregion Coding rule : Function
}
#endif