#if UNITY_EDITOR
using Tools;
using UnityEditor;
using UnityEngine;

public class AddressableBuildGeneratorEditorWindow : BaseEditorWindow
{
    private const float width = 600f;
    private const float height = 200f;
    private const float spacing = 5f;

    private AddressableBuildGenerator addressableBuildGenerator = new();

    private string AdressableAssetPath => GetParameter<string>("AddressableAssetPath");

    [MenuItem("Tools/Addressable/Addresable Build Window")]
    public static void OpenAddresableBuildGenerator()
    {
        AddressableBuildGeneratorEditorWindow window = (AddressableBuildGeneratorEditorWindow)GetWindow(typeof(AddressableBuildGeneratorEditorWindow));
        window.InitializeWindow(window, width, height, spacing);
    }

    protected override void InitializeParameters()
    {
        AddLabel($"Current Addressable Build Path : {addressableBuildGenerator.GetLocalBuildPath()}");

        AddParameter("AddressableAssetPath", PathDefine.Addressable);
    }

    protected override void DrawActionButton()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Update Addressable Entry", GUILayout.Width(Screen.width * 0.3f), GUILayout.Height(50f)))
        {
            if (addressableBuildGenerator != null)
            {
                addressableBuildGenerator.UpdateEntry(AdressableAssetPath);
                Close();
            }
        }

        if (GUILayout.Button("Generate Addressable Build", GUILayout.Width(Screen.width * 0.3f), GUILayout.Height(50f)))
        {
            if (addressableBuildGenerator != null)
            {
                addressableBuildGenerator.Generate(AdressableAssetPath);
                Close();
            }
        }

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
    }

    [MenuItem("Tools/Addressable/Update Entry %F2")]
    public static void UpdateAddressableEntry()
    {
        AddressableBuildGenerator generator = new();
        generator.UpdateEntry("Assets/Addressable");
    }
}
#endif