using System.Collections.Generic;
using TowerOfInfinity.CharacterBuilder; // AddressableSpriteData를 사용하기 위함
using UnityEditor;
using UnityEngine;

public class AddressableSpriteDataImporter : EditorWindow
{
    private TextAsset jsonFile; // 사용자가 .json 파일을 드래그 앤 드롭할 수 있도록
    private string jsonString = ""; // 또는 JSON을 직접 붙여넣을 수 있도록
    private CharacterSpritePartsData targetAddressableData;

    [MenuItem("Tools/Import SpriteCollection to AddressableSpriteData")]
    public static void ShowWindow()
    {
        GetWindow<AddressableSpriteDataImporter>("AddressableSpriteData Importer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Import SpriteCollection JSON to AddressableSpriteData", EditorStyles.boldLabel);

        targetAddressableData = (CharacterSpritePartsData)EditorGUILayout.ObjectField("Target Addressable Data", targetAddressableData, typeof(CharacterSpritePartsData), false);

        jsonFile = (TextAsset)EditorGUILayout.ObjectField("JSON File (Optional)", jsonFile, typeof(TextAsset), false);

        if (jsonFile != null)
        {
            jsonString = jsonFile.text;
        }

        GUILayout.Label("Or paste JSON string directly:");
        jsonString = EditorGUILayout.TextArea(jsonString, GUILayout.Height(position.height / 2 - 100));

        if (GUILayout.Button("Import"))
        {
            if (targetAddressableData == null)
            {
                EditorUtility.DisplayDialog("Error", "Please assign a Target Addressable Data asset.", "OK");
                return;
            }
            if (string.IsNullOrEmpty(jsonString))
            {
                EditorUtility.DisplayDialog("Error", "Please provide JSON data (either via file or text area).", "OK");
                return;
            }

            ImportFromJson(jsonString, targetAddressableData);
        }
    }

    private void ImportFromJson(string json, CharacterSpritePartsData addressableData)
    {
        try
        {

            ExportableSpriteCollectionData importData = JsonUtility.FromJson<ExportableSpriteCollectionData>(json);
            addressableData.LayerEntries.Clear();

            foreach (var exportLayer in importData.layers)
            {
                if (string.IsNullOrEmpty(exportLayer.name))
                {
                    Debug.LogWarning("Skipping layer with empty name in JSON data.");
                    continue;
                }

                var newLayerEntry = new CharacterSpritePartsData.LayerEntry // LayerEntry로 변경
                {
                    LayerName = exportLayer.name,
                    Parts = new List<CharacterSpritePartsData.PartData>() // Parts로 변경
                };

                if (exportLayer.textureNames != null)
                {
                    foreach (var textureName in exportLayer.textureNames)
                    {
                    newLayerEntry.Parts.Add(new CharacterSpritePartsData.PartData
                    {
                        PartName = textureName,
                        Address = string.Format(PathDefine.CHARACTER_BUILDER_PARTS_FORMAT, exportLayer.name, textureName)
                    });
                    }
                }
                addressableData.LayerEntries.Add(newLayerEntry); // LayerEntries 리스트에 추가
            }
            EditorUtility.SetDirty(addressableData); // 에셋을 변경되었음을 표시하여 저장되도록 합니다.
            AssetDatabase.SaveAssets(); // 에셋을 저장합니다.

            EditorUtility.DisplayDialog("Import Complete", "AddressableSpriteData가 JSON 데이터로 채워졌습니다.", "OK");
            Debug.Log("AddressableSpriteData populated from JSON.");
        }
        catch (System.Exception e)
        {
            EditorUtility.DisplayDialog("Error", $"가져오기 중 오류가 발생했습니다: {e.Message}", "OK");
            Debug.LogError($"JSON을 AddressableSpriteData로 가져오는 중 오류 발생: {e.Message}");
        }
    }
}

// SpriteCollectionJsonExporter에서 사용된 동일한 데이터 구조를 재사용합니다.
// 이 클래스들이 이 스크립트와 독립적으로 작동하도록 여기에 다시 정의합니다.
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