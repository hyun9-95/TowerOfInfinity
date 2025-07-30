using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [System.Serializable]
    public struct KeyValue
    {
        public TKey Key;
        public TValue Value;

        public KeyValue(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
    }

    [SerializeField]
    private List<KeyValue> keyValues = new List<KeyValue>();

    public void OnBeforeSerialize()
    {
        keyValues.Clear();

        foreach (KeyValuePair<TKey, TValue> pair in this)
        {
            keyValues.Add(new KeyValue(pair.Key, pair.Value));
        }
    }

    public void OnAfterDeserialize()
    {
        this.Clear();

        for (int i = 0; i < keyValues.Count; i++)
            this.Add(keyValues[i].Key, keyValues[i].Value);
    }
}