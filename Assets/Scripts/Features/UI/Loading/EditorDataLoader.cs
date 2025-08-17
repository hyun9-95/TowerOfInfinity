using UnityEngine;
using Cysharp.Threading.Tasks;
using System.IO;
using System.Linq;

public class EditorDataLoader : BaseDataLoader
{
    private string localJsonDataPath;

    public void SetLocalJsonDataPath(string localJsonDataPathValue)
    {
        localJsonDataPath = localJsonDataPathValue;
    }

    public async override UniTask LoadData()
    {
        bool loadDataResult = await LoadDataFromLocalPath(localJsonDataPath);

        ChangeState(loadDataResult ? State.Success : State.Fail);
    }

    private async UniTask<bool> LoadDataFromLocalPath(string jsonPath)
    {
        if (!Directory.Exists(jsonPath))
        {
            ChangeState(State.Fail);
            return false;
        }

        string[] localJsonFileNames = Directory.GetFiles(jsonPath, $"*.json")
            .Select(Path.GetFileName)
            .ToArray();

        if (!localJsonFileNames.IsValidArray())
        {
            ChangeState(State.Fail);
            return false;
        }

        float progressIncrementValue = 1f / localJsonFileNames.Length;

        ChangeState(State.LoadJson);

        foreach (string fileName in localJsonFileNames)
        {
            bool result = await LoadJsonToDic(Path.Combine(jsonPath, fileName), progressIncrementValue);
            if (!result)
            {
                ChangeState(State.Fail);
                return false;
            }
        }

        return true;
    }

    private async UniTask<bool> LoadJsonToDic(string filePath, float progressIncrementValue)
    {
        string localJson = null;

        await UniTask.RunOnThreadPool(() => { localJson = File.ReadAllText(filePath); });

        if (string.IsNullOrEmpty(localJson))
        {
            return false;
        }

        DicJsonByFileName.Add(Path.GetFileName(filePath), localJson);

        CurrentProgressValue += progressIncrementValue;

        return true;
    }
}
