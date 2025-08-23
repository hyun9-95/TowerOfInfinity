using UnityEngine;

public abstract class BaseMonoManager<T> : MonoBehaviour where T : BaseMonoManager<T>, new()
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
                instance = FindFirstObjectByType<T>();

            return instance;
        }
    }

    protected void SetInstance(T instanceValue) 
    {
        instance = instanceValue;
    }
}
