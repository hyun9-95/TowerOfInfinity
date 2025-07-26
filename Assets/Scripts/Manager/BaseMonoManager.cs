using UnityEngine;

public abstract class BaseMonoManager<T> : MonoBehaviour where T : BaseMonoManager<T>, new()
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = GameObject.Find(typeof(T).Name);

                if (go == null)
                    return null;

                instance = go.GetComponent<T>();
            }

            return instance;
        }
    }
}
