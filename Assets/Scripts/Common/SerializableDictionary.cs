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

        foreach (KeyValuePair<TKey, TValue> pair in this)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        this.Clear(); // 딕셔너리를 명확하게 비웁니다.
        if (keys.Count != values.Count)
        {
            Debug.LogError("There are " + keys.Count + " keys and " + values.Count + " values after deserialization. Make sure that both key and value types are serializable.");
        }
        for (int i = 0; i < keys.Count; i++)
        {
            // 중복 키를 방지하고, 기존 값을 덮어쓰거나 새로 추가합니다.
            if (this.ContainsKey(keys[i]))
            {
                this[keys[i]] = values[i];
            }
            else
            {
                this.Add(keys[i], values[i]);
            }
        }
    }
}
