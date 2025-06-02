#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Tools
{
    public class DataGeneratorEditorWindow : BaseEdtiorWindow
    {
        private const float width = 400f;
        private const float height = 200f;
        private const float spacing = 5f;

        private string ExcelPath => GetParameter<string>("ExcelPath");
        private string JsonPath => GetParameter<string>("JsonPath");
        private int Version => GetParameter<int>("Version");
        

        [MenuItem("Tools/Data/Generate Data From Excel Folder")]
        public static void OpenDataGeneratorWindow()
        {
            DataGeneratorEditorWindow window = (DataGeneratorEditorWindow)GetWindow(typeof(DataGeneratorEditorWindow));
            window.InitializeWindow(window, width, height, spacing);
        }

        protected override void InitializeParameters()
        {
            AddParameter("ExcelPath", PathDefine.Excel);
            AddParameter("JsonPath", PathDefine.Json);
            AddParameter("Version", 0);
        }

        protected override void DrawActionButton()
        {
            if (GUILayout.Button("Generate", GUILayout.Height(50)))
            { 
                DataGenerator.GenerateDataFromExcelFoler(ExcelPath, JsonPath, Version);
                Close();
            }
        }
    }
}
#endif
