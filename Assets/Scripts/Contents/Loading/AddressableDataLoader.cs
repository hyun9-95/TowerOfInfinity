using Cysharp.Threading.Tasks;
using UnityEngine;

public class AddressableDataLoader : BaseDataLoader
{
    public GameObject AddressableTracker { get; private set; }

    /// <summary>
    /// 모두 로드 후에 ReleaseGameObject로 텍스트에셋들 해제
    /// </summary>
    /// <param name="tracker"></param>
    public void SetAddressableTracker(GameObject tracker)
    {
        AddressableTracker = tracker;
    }

    public async override UniTask LoadData()
    {
        bool loadDataResult = await LoadDataFromAddressable();
        
        ChangeState(loadDataResult ? State.Success : State.Fail);
    }

    private async UniTask<bool> LoadDataFromAddressable()
    {
        ChangeState(State.LoadJsonList);
        
        string[] jsonFileNames = await LoadJsonList();
        
        if (!jsonFileNames.IsValidArray())
        {
            ChangeState(State.Fail);
            return false;
        }

        float progressIncrementValue = 1f / jsonFileNames.Length;
        
        ChangeState(State.LoadJson);
        
        foreach (string fileName in jsonFileNames)
        {
            bool result = await LoadJsonFromAddressable(fileName, progressIncrementValue);
            if (!result)
            {
                ChangeState(State.Fail);
                return false;
            }
        }
        
        return true;
    }

    private async UniTask<string[]> LoadJsonList()
    {
        TextAsset jsonListAsset = await AddressableManager.Instance.LoadAssetAsyncWithTracker<TextAsset>
            (PathDefine.PATH_ADDRESSABLE_JSON_LIST, AddressableTracker);
        
        if (jsonListAsset == null)
        {
            Logger.Null("JsonList.txt");
            return null;
        }

        string jsonListText = jsonListAsset.text;
        
        if (string.IsNullOrEmpty(jsonListText))
        {
            Logger.Error("JsonList.txt is Empty");
            return null;
        }

        return jsonListText.Split(',');
    }

    private async UniTask<bool> LoadJsonFromAddressable(string fileName, float progressIncrementValue)
    {
        string fileNameWithoutExtension = fileName.Replace(".json", "");
        string addressableKey = string.Format(PathDefine.PATH_ADDRESSABLE_JSON, fileNameWithoutExtension);

        TextAsset jsonAsset = await AddressableManager.Instance.LoadAssetAsyncWithTracker<TextAsset>(addressableKey, AddressableTracker);
        
        if (jsonAsset == null)
            return false;

        string jsonContent = jsonAsset.text;
        
        if (string.IsNullOrEmpty(jsonContent))
            return false;

        DicJsonByFileName.Add(fileName, jsonContent);
        CurrentProgressValue += progressIncrementValue;
        
        return true;
    }
}