#pragma warning disable CS0162
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : BaseMonoManager<ObjectPoolManager>
{
    [SerializeField]
    private Transform poolRoot;

    private Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();
    private Dictionary<string, Transform> poolParentDictionary = new Dictionary<string, Transform>();
    private Dictionary<int, Vector3> originScaleDic = new Dictionary<int, Vector3>();

    private Transform GetPoolParent(string key)
    {
        if (!poolParentDictionary.TryGetValue(key, out Transform parent))
        {
            parent = new GameObject(key).transform;
            parent.SetParent(poolRoot);
            poolParentDictionary[key] = parent;
        }

        return parent;
    }

    private async UniTask<GameObject> Spawn(string name, Vector3 position = default, Quaternion rotation = default)
    {
        if (!poolDictionary.ContainsKey(name))
            poolDictionary[name] = new Queue<GameObject>();

        GameObject go;

        if (poolDictionary[name].Count > 0)
        {
            go = poolDictionary[name].Dequeue();
            go.transform.SetPositionAndRotation(position, rotation);

            if (originScaleDic.TryGetValue(go.GetInstanceID(), out Vector3 originScale))
                go.transform.localScale = originScale;

            go.SetActive(true);
        }
        else
        {
            go = await AddressableManager.Instance.InstantiateAsync(name, GetPoolParent(name));
            go.name = name;

            if (go == null)
                return null;

            go.transform.SetPositionAndRotation(position, rotation);
            originScaleDic[go.GetInstanceID()] = go.transform.localScale;
        }

        return go;
    }

    public async UniTask<T> SpawnPoolableMono<T>(string name, Vector3 position = default, Quaternion rotation = default)
        where T : PoolableMono
    {
        var go = await Spawn(name, position, rotation);

        if (go == null)
            return null;

        return go.GetComponent<T>();
    }

    public void Clear()
    {
        poolDictionary.Clear();

        foreach (var parent in poolParentDictionary.Values)
        {
            if (parent != null)
                GameObject.Destroy(parent.gameObject);
        }

        poolParentDictionary.Clear();
        originScaleDic.Clear();
    }

    public void ReturnToPool(GameObject obj, string name)
    {
        obj.SetActive(false);

        if (!poolDictionary.ContainsKey(name))
            return;

        poolDictionary[name].Enqueue(obj);
    }
}