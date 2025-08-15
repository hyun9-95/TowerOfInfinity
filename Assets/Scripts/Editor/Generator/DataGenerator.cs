#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using ExcelDataReader;
using System.Data;
using System.Linq;

namespace Tools
{
    public static class DataGenerator
    {
        private const string ExcelFileExtension = ".xlsx";
        private const string CsvFileExtension = ".csv";

        //반복사용하는 generator만 빼놓음
        private static readonly ClassGenerator structGenerator = new();
        private static readonly JsonGenerator jsonGenerator = new();
        private static readonly DataDefineGenerator dataDefineGenerator = new();
        private static readonly CsvReader csvReader = new();
        
        private static float progress;
        private static string excelFolderPath;
        private static string jsonFolderPath;
        private static int version;

        public static void GenerateDataFromExcelFolder(string excelFolderPathValue, string jsonFolderPathValue, int versionValue)
        {
            GenerateDataFromFiles(excelFolderPathValue, jsonFolderPathValue, versionValue, ExcelFileExtension);
        }

        public static void GenerateDataFromCsvFolder(string csvFolderPathValue, string jsonFolderPathValue, int versionValue)
        {
            GenerateDataFromFiles(csvFolderPathValue, jsonFolderPathValue, versionValue, CsvFileExtension);
        }

        private static void GenerateDataFromFiles(string folderPathValue, string jsonFolderPathValue, int versionValue, string fileExtension)
        {
            if (!AssetDatabase.IsValidFolder(folderPathValue))
            {
                Logger.Error("Invalid folder path");
                return;
            }

            if (string.IsNullOrEmpty(jsonFolderPathValue))
            {
                Logger.Null("Json save path");
                return;
            }

            Initialize(folderPathValue, jsonFolderPathValue, versionValue);

            string[] dataFiles = Directory.GetFiles(excelFolderPath, $"*{fileExtension}");

            if (dataFiles.Length == 0)
            {
                Logger.Error($"There is no {fileExtension} file in {excelFolderPath}.");
                return;
            }

            try
            {
                if (fileExtension == ExcelFileExtension)
                {
                    GenerateDataFromExcelFilePaths(dataFiles);
                }
                else if (fileExtension == CsvFileExtension)
                {
                    GenerateDataFromCsvFilePaths(dataFiles);
                }
                
                GenerateContainerManager();
                GenerateJsonList();
                GenerateVersion();
            }
            catch (Exception e)
            {
                Logger.Exception($"Error occured while generate data. => {folderPathValue}", e);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
                AssetDatabase.Refresh();
            }
        }

        private static void Initialize(string folderPathValue, string jsonPathValue, int versionValue)
        {
            excelFolderPath = folderPathValue;
            jsonFolderPath = $"Assets/{jsonPathValue}";
            version = versionValue;

            progress = 0f;
        }

        /// <summary> 엑셀파일이 열려있으면 에러 발생 </summary>
        public static void GenerateDataFromExcelFilePaths(string[] excelFiles)
        {
            Logger.Log("----------------Check Excel Start-----------------");
            for (int i = 0; i < excelFiles.Length; i++)
            {
                string excelPath = excelFiles[i];

                EditorUtility.DisplayProgressBar(excelFolderPath, $"Converting {excelPath}..", progress);
                GenerateDataFromExcelPath(excelPath.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
                progress += 1f / excelFiles.Length;
            }
            Logger.Log("----------------Check Excel End------------------");
        }

        /// <summary> CSV 파일 처리 </summary>
        public static void GenerateDataFromCsvFilePaths(string[] csvFiles)
        {
            Logger.Log("----------------Check CSV Start-----------------");
            for (int i = 0; i < csvFiles.Length; i++)
            {
                string csvPath = csvFiles[i];

                EditorUtility.DisplayProgressBar(excelFolderPath, $"Converting {csvPath}..", progress);
                GenerateDataFromCsvPath(csvPath.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
                progress += 1f / csvFiles.Length;
            }
            Logger.Log("----------------Check CSV End------------------");
        }

        private static bool GenerateDataFromExcelPath(string readExcelPath)
        {
            using (FileStream fileStream = File.Open(readExcelPath, FileMode.Open, FileAccess.Read))
            using (IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(fileStream))
            {
                var dataSet = excelReader.AsDataSet();

                //시트는 하나만 사용할 것
                DataTable sheet = dataSet.Tables[0];

                string excelFileName = Path.GetFileName(readExcelPath);

                GenerateStructFromExcelSheet(readExcelPath, sheet);
                GenerateJsonFromExcelSheet(readExcelPath, sheet);
                GenerateDataDefineFromExcelSheet(readExcelPath, sheet);

                return true;
            }
        }

        private static bool GenerateDataFromCsvPath(string readCsvPath)
        {
            try
            {
                DataTable sheet = csvReader.ReadCsvToDataTable(readCsvPath);

                if (sheet == null)
                {
                    Logger.Error($"Failed to read CSV file: {readCsvPath}");
                    return false;
                }

                GenerateStructFromExcelSheet(readCsvPath, sheet);
                GenerateJsonFromExcelSheet(readCsvPath, sheet);
                GenerateDataDefineFromExcelSheet(readCsvPath, sheet);

                return true;
            }
            catch (Exception e)
            {
                Logger.Exception($"Error processing CSV file: {readCsvPath}", e);
                return false;
            }
        }

        private static void GenerateStructFromExcelSheet(string readExcelPath, DataTable sheet)
        {
            structGenerator.Init(readExcelPath);
            structGenerator.Generate(sheet);
        }

        private static void GenerateJsonFromExcelSheet(string readExcelPath, DataTable sheet)
        {
            jsonGenerator.Init(readExcelPath, jsonFolderPath);
            jsonGenerator.Generate(sheet);
        }

        private static void GenerateDataDefineFromExcelSheet(string readExcelPath, DataTable sheet)
        {
            dataDefineGenerator.Init(readExcelPath);
            dataDefineGenerator.Generate(sheet);
        }

        private static void GenerateContainerManager()
        {
            EditorUtility.DisplayProgressBar(PathDefine.Manager, $"Writing DataContainerManager.cs..", progress);

            string absoluteJsonPath = Path.Combine(UnityEngine.Application.dataPath.Replace("Assets", ""), jsonFolderPath);
            Logger.Log($"GenerateContainerManager: absoluteJsonPath = {absoluteJsonPath}");
            string[] dataNames = Directory.GetFiles(absoluteJsonPath, "*.json").Select(x => $"Data{Path.GetFileNameWithoutExtension(x)}").ToArray();
            
            DataContainerGeneratorGenerator containerManagerGenerator = new();
            containerManagerGenerator.Generate(dataNames);
        }

        private static void GenerateJsonList()
        {
            EditorUtility.DisplayProgressBar("Finishing", $"Writing JsonList.txt..", progress);

            string absoluteJsonPath = Path.Combine(UnityEngine.Application.dataPath.Replace("Assets", ""), jsonFolderPath);
            JsonListGenerator jsonListGenerator = new ();
            jsonListGenerator.Generate(absoluteJsonPath, jsonFolderPath);
        }

        private static void GenerateVersion()
        {
            EditorUtility.DisplayProgressBar("Finishing", $"Writing Version.txt..", progress);

            VersionTextGenerator versionTextGenerator = new ();
            versionTextGenerator.Generate(jsonFolderPath, version.ToString());
        }
    }
}
#endif