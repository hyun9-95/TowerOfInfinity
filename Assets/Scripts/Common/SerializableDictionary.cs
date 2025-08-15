using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField]
    private List<TKey> keys = new List<TKey>();
    
    [SerializeField]
    private List<TValue> values = new List<TValue>();

    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();
        
        foreach (var kvp in this)
        {
            keys.Add(kvp.Key);
            values.Add(kvp.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        Clear();
        
        if (keys != null && values != null)
        {
            int count = Mathf.Min(keys.Count, values.Count);
            for (int i = 0; i < count; i++)
            {
                if (keys[i] != null && !ContainsKey(keys[i]))
                {
                    Add(keys[i], values[i]);
                }
            }
        }
    }

    // Inspector에서 수동으로 키-값을 추가할 수 있도록 하는 메소드
    [ContextMenu("Add New Entry")]
    public void AddNewEntry()
    {
        keys.Add(default(TKey));
        values.Add(default(TValue));
    }

    // Inspector에서 보이는 키-값 쌍의 개수
    public int InspectorCount => keys != null ? keys.Count : 0;
    
    // Inspector에서 특정 인덱스의 키-값에 접근
    public TKey GetKeyAt(int index)
    {
        return keys != null && index >= 0 && index < keys.Count ? keys[index] : default(TKey);
    }
    
    public TValue GetValueAt(int index)
    {
        return values != null && index >= 0 && index < values.Count ? values[index] : default(TValue);
    }
    
    public void SetKeyAt(int index, TKey key)
    {
        if (keys != null && index >= 0 && index < keys.Count)
            keys[index] = key;
    }
    
    public void SetValueAt(int index, TValue value)
    {
        if (values != null && index >= 0 && index < values.Count)
            values[index] = value;
    }
}
