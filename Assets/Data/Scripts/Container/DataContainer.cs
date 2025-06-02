using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class DataContainer<T> where T : IBaseData
{
    private readonly Dictionary<int, T> dicById = new();
    private readonly Dictionary<string, T> dicByNameId = new();
    private List<string> propertyNames = null;
    private T[] datas = null;

    public bool Deserialized => dicById != null && dicByNameId != null && datas != null;
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

    public T GetByNameId(string nameId)
    {
        if (string.IsNullOrEmpty(nameId))
            return default;

        if (dicByNameId.ContainsKey(nameId))
            return dicByNameId[nameId];

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

    public List<string> GetAllPropertyNames()
    {
        return propertyNames;
    }

    private bool AddDatas(JArray array, bool initPropertyNames = false)
    {
        foreach (var jObj in array)
        {
            T data = JsonConvert.DeserializeObject<T>(jObj.ToString());

            if (!TryAddData(data))
                return false;
        }

        datas = dicById.Values.ToArray();

        if (initPropertyNames)
            InitPropertyNames();

        return true;
    }

    private bool TryAddData(T data)
    {
        if (data.NameId is null)
        {
            Logger.Null($"{typeof(T)}");
            return false;
        }

        if (!dicById.TryAdd(data.Id, data))
        {
            Logger.Error($"Duplicated Id : {data.GetType()} / {data.Id}");
            return false;
        }

        if (!dicByNameId.TryAdd(data.NameId, data))
        {
            Logger.Error($"Duplicated NameId : {data.GetType()} / {data.NameId}");
            return false;
        }

        return true;
    }

    private void InitPropertyNames()
    {
        propertyNames = new();

        Type type = typeof(T);

        PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (PropertyInfo property in properties)
            propertyNames.Add(property.Name);

        FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance );
        foreach (FieldInfo field in fields)
            propertyNames.Add(field.Name);
    }
}
