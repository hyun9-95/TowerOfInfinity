using UnityEditor;
using UnityEngine;
using System.IO;
using Assets.PixelFantasy.PixelHeroes.Common.Scripts.CollectionScripts; // SpriteCollection 네임스페이스

public class SpriteCollectionJsonExporter : EditorWindow
{
    private SpriteCollection targetCollection;
    private string outputPath = "Assets/SpriteCollectionData.json";

    [MenuItem("Tools/Export SpriteCollection to JSON")]
    public static void ShowWindow()
    {
        GetWindow<SpriteCollectionJsonExporter>("SpriteCollection JSON Exporter");
    }

    private void OnGUI()
    {
        GUILayout.Label("Export SpriteCollection Data", EditorStyles.boldLabel);

        targetCollection = (SpriteCollection)EditorGUILayout.ObjectField("Sprite Collection", targetCollection, typeof(SpriteCollection), false);

        outputPath = EditorGUILayout.TextField("Output Path", outputPath);

        if (GUILayout.Button("Export"))
        {
            if (targetCollection == null)
            {
                EditorUtility.DisplayDialog("Error", "Please assign a SpriteCollection asset.", "OK");
                return;
            }

            ExportToJson(targetCollection, outputPath);
        }
    }

    private void ExportToJson(SpriteCollection collection, string path)
    {
        // Unity의 JsonUtility는 복잡한 참조를 제대로 직렬화하지 못할 수 있습니다.
        // 특히 Texture2D와 같은 Object 참조는 경로로만 직렬화됩니다.
        // 여기서는 Layers와 Textures의 이름만 추출하여 JSON으로 만듭니다.
        // 실제 텍스처 데이터는 포함되지 않습니다.

        var exportData = new ExportableSpriteCollectionData();
        exportData.name = collection.name;

        foreach (var layer in collection.Layers)
        {
            var exportLayer = new ExportableLayerData
            {
                name = layer.Name,
                textureNames = new System.Collections.Generic.List<string>()
            };

            if (layer.Textures != null)
            {
                foreach (var texture in layer.Textures)
                {
                    if (texture != null)
                    {
                        exportLayer.textureNames.Add(texture.name);
                    }
                }
            }
            exportData.layers.Add(exportLayer);
        }

        string json = JsonUtility.ToJson(exportData, true);
        File.WriteAllText(path, json);

        EditorUtility.DisplayDialog("Export Complete", $"SpriteCollection data exported to {path}", "OK");
        Debug.Log($"SpriteCollection data exported to {path}");
    }

    // JsonUtility가 직렬화할 수 있도록 별도의 클래스 정의
    [System.Serializable]
    public class ExportableSpriteCollectionData
    {
        public string name;
        public System.Collections.Generic.List<ExportableLayerData> layers = new System.Collections.Generic.List<ExportableLayerData>();
    }

    [System.Serializable]
    public class ExportableLayerData
    {
        public string name;
        public System.Collections.Generic.List<string> textureNames;
    }
}