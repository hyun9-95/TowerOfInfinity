using System;
using System.Collections.Generic;
using System.Linq;

public class DataManager : BaseManager<DataManager>
{
    private Dictionary<Type, object> containerDic = new();

    public Type[] GetAllTypes() => containerDic.Keys.ToArray();

    public DataContainer<T> GetDataContainer<T>() where T : IBaseData
    {
        return containerDic.ContainsKey(typeof(T)) ? (DataContainer<T>)containerDic[typeof(T)] : null;
    }

    public T GetDataById<T>(int id) where T : IBaseData
    {
        return GetDataContainer<T>() != null ? GetDataContainer<T>().GetById(id) : default;
    }

    public T FindData<T>(Predicate<T> predicate) where T : IBaseData
    {
        return GetDataContainer<T>() != null ? GetDataContainer<T>().Find(predicate) : default;
    }

    public T[] FindAllData<T>(Predicate<T> predicate) where T : IBaseData
    {
        return GetDataContainer<T>() != null ? GetDataContainer<T>().FindAll(predicate) : default;
    }

    public T[] GetAllData<T>() where T : IBaseData
    {
        return GetDataContainer<T>() != null ? GetDataContainer<T>().FindAll(x => !x.IsEmpty) : default;
    }

    public bool GenerateDataContainerByDataDic(Dictionary<string, string> dicJsonByFileName)
    {
        DataContainerGenerator containerGenerator = new DataContainerGenerator();
        containerDic = containerGenerator.GenerateDataContainerByDataDic(dicJsonByFileName);

        return containerDic != null;
    }
}