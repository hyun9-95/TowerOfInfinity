using System.Collections.Generic;
using System;

public class DataContainerGenerator
{
    private Dictionary<Type, object> containerDic;
    private Dictionary<Type, Func<string, bool>> containerTypeMap;

    public Dictionary<Type, object> GenerateDataContainerByDataDic(Dictionary<string, string> dicJsonByFileName)
    {
        containerDic = new Dictionary<Type, object>();

        foreach (string fileName in dicJsonByFileName.Keys)
        {
            bool result = AddDataContainer(fileName, dicJsonByFileName[fileName]);

            if (!result)
                return null;
        }

        return containerDic;
    }

    public bool AddDataContainer<T>(string json) where T : IBaseData
    {
        if (containerDic.ContainsKey(typeof(T)))
        {
            Logger.Error($"Duplicated DataContainer type : {typeof(T)}");
            return false;
        }

        DataContainer<T> dataContainer = new();

        if (dataContainer.DeserializeJson(json))
        {
            containerDic[typeof(T)] = dataContainer;
            Logger.Success($"Add DataContainer type : {typeof(T)}");
            return true;
        }

        return false;
    }

    public bool AddDataContainer(string fileName, string json)
    {
        if (fileName.Contains(".json"))
            fileName = fileName.Replace(".json", "");

        Type type = Type.GetType($"Data{fileName}");

        if (containerTypeMap.TryGetValue(type, out var addContainerFunc))
        {
            return addContainerFunc(json);
        }

        Logger.Error($"Invalid Type : {fileName}");
        return false;
    }

    public DataContainerGenerator()
	{
		containerDic = new Dictionary<Type, object>();
		containerTypeMap = new Dictionary<Type, Func<string, bool>>
		{
			{ typeof(DataAbility), json => AddDataContainer<DataAbility>(json) },
			{ typeof(DataBalance), json => AddDataContainer<DataBalance>(json) },
			{ typeof(DataBattleEvent), json => AddDataContainer<DataBattleEvent>(json) },
			{ typeof(DataCharacter), json => AddDataContainer<DataCharacter>(json) },
			{ typeof(DataCharacterParts), json => AddDataContainer<DataCharacterParts>(json) },
			{ typeof(DataDungeon), json => AddDataContainer<DataDungeon>(json) },
			{ typeof(DataEnemyGroup), json => AddDataContainer<DataEnemyGroup>(json) },
			{ typeof(DataLocalization), json => AddDataContainer<DataLocalization>(json) },
		};
	}

}

	
