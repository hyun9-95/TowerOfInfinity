#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Tools
{
    public class DataGeneratorEditorWindow : BaseEdtiorWindow
    {
        private const float width = 400f;
        private const float height = 300f;
        private const float spacing = 5f;

        private string ExcelPath => GetParameter<string>("ExcelPath");
        private string JsonPath => GetParameter<string>("JsonPath");
        private int Version => GetParameter<int>("Version");
        private bool UseExcel => GetParameter<bool>("UseExcel");
        

        [MenuItem("Tools/Data/Generate Data From Files  %F3")]
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
            AddParameter("UseExcel", true);
        }

        protected override void DrawActionButton()
        {
            // File type selection
            GUILayout.BeginHorizontal();
            GUILayout.Label("File Type:", GUILayout.Width(80));
            
            bool newUseExcel = GUILayout.Toggle(UseExcel, "Excel (.xlsx)", GUILayout.Width(120));
            bool useCsv = GUILayout.Toggle(!UseExcel, "CSV (.csv)", GUILayout.Width(120));
            
            if (newUseExcel != UseExcel)
                SetParameter("UseExcel", newUseExcel);
            else if (useCsv == UseExcel)
                SetParameter("UseExcel", false);
                
            GUILayout.EndHorizontal();
            
            GUILayout.Space(10);
            
            // Generate button
            string buttonText = UseExcel ? "Generate from Excel" : "Generate from CSV";
            if (GUILayout.Button(buttonText, GUILayout.Height(50)))
            { 
                if (UseExcel)
                {
                    DataGenerator.GenerateDataFromExcelFolder(ExcelPath, JsonPath, Version);
                }
                else
                {
                    DataGenerator.GenerateDataFromCsvFolder(ExcelPath, JsonPath, Version);
                }
                Close();
            }
        }
    }
}
#endif
