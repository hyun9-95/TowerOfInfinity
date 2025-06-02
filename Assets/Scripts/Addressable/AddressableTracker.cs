using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableTracker : MonoBehaviour
{
    private static List<AsyncOperationHandle> GameObjectHandle { get; set; } = new List<AsyncOperationHandle>();
    private static Dictionary<Object, List<AsyncOperationHandle>> ResourcesHandle { get; set; } = new Dictionary<Object, List<AsyncOperationHandle>>();

    public static void AddGameObjectHandle<T>(AsyncOperationHandle<T> handle) where T : Object
    {
        GameObjectHandle.Add(handle);
    }

    public static void AddResourcesHandle<T>(string key, Object obj, AsyncOperationHandle<T> handle)
    {
        if (!ResourcesHandle.ContainsKey(obj))
            ResourcesHandle.Add(obj, new List<AsyncOperationHandle>());

        ResourcesHandle[obj].Add(handle);
    }

    public static void RelaseHandle()
    {
        RelaseGameObjectHandle();

        Dictionary<Object, List<AsyncOperationHandle>> tempHandles = new Dictionary<Object, List<AsyncOperationHandle>>();

        foreach (var handle in ResourcesHandle)
        {
            if (handle.Key == null ||
                handle.Key.Equals(null))
            {
                for (int i = handle.Value.Count - 1; i >= 0; --i)
                    handle.Value[i].Release();

                handle.Value.Clear();
            }
            else
            {
                tempHandles.Add(handle.Key, handle.Value);
            }
        }

        ResourcesHandle = tempHandles;
    }

    public static void RelaseGameObjectHandle()
    {
        for (int i = GameObjectHandle.Count - 1; i >= 0; --i)
        {
            if (GameObjectHandle[i].IsValid())
            {
                if (GameObjectHandle[i].Result == null ||
                    GameObjectHandle[i].Result.Equals(null))
                {
                    GameObjectHandle[i].Release();
                    GameObjectHandle.RemoveAt(i);
                }
                else
                {
                    GameObject gameObject = GameObjectHandle[i].Result as GameObject;

                    if (gameObject == null)
                        continue;

                    if (string.IsNullOrEmpty(gameObject.scene.name))
                    {
                        GameObjectHandle[i].Release();
                        GameObjectHandle.RemoveAt(i);
                    }
                }
            }
            else
            {
                GameObjectHandle.RemoveAt(i);
            }
        }
    }

    [ContextMenu("RELASE")]
    public void TestRelase()
    {
        RelaseHandle();
    }
}