using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

public class DataContainer<T> where T : IBaseData
{
    private readonly Dictionary<int, T> dicById = new();
    private T[] datas = null;

    public bool Deserialized => dicById != null && datas != null;
    public bool DeserializeJson(string json)
    {
        if (string.IsNullOrEmpty(json))
        {
            Logger.Null($"{typeof(T)}");
            return false;
        }

        try
        {
            JArray jArray = JArray.Parse(json);
            return AddDatas(jArray);
        }
        catch (Exception e)
        {
            Logger.Exception("Json parsing failed", e);
            return false;
        }
    }

    public T GetById(int id)
    {
        if (dicById.ContainsKey(id))
            return dicById[id];

        return default;
    }


    public T Find(Predicate<T> predicate)
    {
        return Array.Find(datas, predicate);
    }

    public T[] FindAll(Predicate<T> predicate)
    {
        return Array.FindAll(datas, predicate);
    }

    private bool AddDatas(JArray array)
    {
        foreach (var jObj in array)
        {
            T data = JsonConvert.DeserializeObject<T>(jObj.ToString());

            if (!TryAddData(data))
                return false;
        }

        datas = dicById.Values.ToArray();
        return true;
    }

    private bool TryAddData(T data)
    {
        if (!dicById.TryAdd(data.Id, data))
        {
            Logger.Error($"Duplicated Id : {data.GetType()} / {data.Id}");
            return false;
        }

        return true;
    }
}
