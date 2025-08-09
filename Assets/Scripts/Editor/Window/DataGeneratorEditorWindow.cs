#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Tools
{
    public class DataGeneratorEditorWindow : BaseEdtiorWindow
    {
        private const float width = 400f;
        private const float height = 400f;
        private const float spacing = 5f;

        private string ExcelPath => GetParameter<string>("ExcelPath");
        private string JsonPath => GetParameter<string>("JsonPath");
        private int Version => GetParameter<int>("Version");
        private bool UseExcel => GetParameter<bool>("UseExcel");
        private bool CheckIntegrity => GetParameter<bool>("CheckIntegrity");
        

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
            AddParameter("CheckIntegrity", true);
        }

        protected override void DrawActionButton()
        {
            DrawFileTypeSection();
            DrawOptionsSection();
            DrawGenerateButton();
        }

        private void DrawFileTypeSection()
        {
            GUILayout.Label("File Source", EditorStyles.boldLabel);
            
            using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("Format:", GUILayout.Width(60));
                
                bool newUseExcel = GUILayout.Toggle(UseExcel, "Excel (.xlsx)", GUILayout.Width(110));
                bool useCsv = GUILayout.Toggle(!UseExcel, "CSV (.csv)", GUILayout.Width(110));
                
                if (newUseExcel != UseExcel)
                    SetParameter("UseExcel", newUseExcel);
                else if (useCsv == UseExcel)
                    SetParameter("UseExcel", false);
            }
            
            GUILayout.Space(10);
        }

        private void DrawOptionsSection()
        {
            GUILayout.Label("Generation Options", EditorStyles.boldLabel);
            
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                bool newCheckIntegrity = EditorGUILayout.Toggle("Verify data integrity after generation", CheckIntegrity);
                if (newCheckIntegrity != CheckIntegrity)
                    SetParameter("CheckIntegrity", newCheckIntegrity);
                
                if (CheckIntegrity)
                {
                    EditorGUILayout.HelpBox("Generated JSON files will be validated for parsing errors and duplicate IDs.", MessageType.Info);
                }
            }
            
            GUILayout.Space(15);
        }

        private void DrawGenerateButton()
        {
            string buttonText = UseExcel ? "Generate from Excel Files" : "Generate from CSV Files";
            string statusText = UseExcel ? "Excel -> JSON" : "CSV -> JSON";
            
            using (new EditorGUILayout.VerticalScope())
            {
                GUILayout.Label($"Action: {statusText}", EditorStyles.centeredGreyMiniLabel);
                
                if (GUILayout.Button(buttonText, GUILayout.Height(40)))
                {
                    ExecuteGeneration();
                }
            }
        }

        private void ExecuteGeneration()
        {
            try
            {
                Logger.Log($"Starting data generation from {(UseExcel ? "Excel" : "CSV")} files...");
                
                if (UseExcel)
                    DataGenerator.GenerateDataFromExcelFolder(ExcelPath, JsonPath, Version);
                else
                    DataGenerator.GenerateDataFromCsvFolder(ExcelPath, JsonPath, Version);
                
                Logger.Log("Data generation completed.");
                
                if (CheckIntegrity)
                {
                    Logger.Log("Starting integrity verification...");
                    IntegrityCheck();
                }
            }
            catch (System.Exception e)
            {
                Logger.Error($"Data generation error: {e.Message}");
            }
            finally
            {
                Close();
            }
        }

        private void IntegrityCheck()
        {
            Logger.Log("Starting data integrity check...");
            
            try
            {
                var results = DataIntegrityChecker.CheckAllJsonFilesStatic(JsonPath);
                
                int successCount = 0;
                int errorCount = 0;
                
                foreach (var result in results)
                {
                    if (result.IsValid)
                        successCount++;
                    else
                        errorCount++;
                        
                    if (!result.IsValid)
                    {
                        Logger.Error($"Data integrity error in {result.FileName}: {result.ErrorMessage}");
                    }
                }
                
                if (errorCount == 0)
                {
                    Logger.Log($"Data integrity check completed successfully. {successCount} files validated.");
                }
                else
                {
                    Logger.Error($"Data integrity check completed with issues: {successCount} success, {errorCount} errors.");
                }
            }
            catch (System.Exception e)
            {
                Logger.Error($"Data integrity check failed: {e.Message}");
            }
        }
    }
}
#endif
