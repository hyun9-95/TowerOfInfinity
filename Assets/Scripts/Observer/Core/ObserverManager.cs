using System;
using System.Collections.Generic;

public class ObserverManager
{
    private static Dictionary<Enum, List<IObserver>> observers = new Dictionary<Enum, List<IObserver>>();

    public static void Clear()
    {
        observers.Clear();
    }

    public static void AddObserver(Enum id, IObserver addObserver)
    {
        if (observers.ContainsKey(id))
        {
            for (int i = 0; i < observers[id].Count; i++)
            {
                if (observers[id][i].GetHashCode() == addObserver.GetHashCode())
                {
                    return;
                }
            }

            observers[id].Add(addObserver);
        }
        else
        {
            List<IObserver> list = new List<IObserver>();
            list.Add(addObserver);
            observers.Add(id, list);
        }
    }

    public static void RemoveObserver(IObserver removeObserver)
    {
        foreach (KeyValuePair<Enum, List<IObserver>> observer in observers)
        {
            if (observer.Value != null && observer.Value.Count != 0)
            {
                observer.Value.Remove(removeObserver);
            }
        }
    }

    public static void RemoveObserver(Enum id)
    {
        if (observers.ContainsKey(id))
        {
            observers.Remove(id);
        }
    }

    public static void RemoveObserver(Enum id, IObserver removeObserver)
    {
        if (observers.ContainsKey(id))
        {
            observers[id].Remove(removeObserver);
        }
    }

    public static void NotifyObserver(Enum id, IObserverParam observerParam)
    {
        if (!observers.ContainsKey(id))
        {
            return;
        }

        for (int i = 0; i < observers[id].Count; i++)
        {
            if (observers[id][i] != null)
            {
                observers[id][i].HandleMessage(id, observerParam);
            }
        }
    }
}