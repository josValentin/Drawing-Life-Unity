using System.Collections.Generic;
using UnityEngine.InputSystem;

[System.Serializable]
public struct SKeyValuePair<K, V>
{
    public K key;
    public V value;

    public SKeyValuePair(K key, V value)
    {
        this.key = key;
        this.value = value;
    }
}

public struct SKeyValuePair
{
    public static SKeyValuePair<K, V> From<K, V>(KeyValuePair<K, V> source) => new SKeyValuePair<K, V>(source.Key, source.Value);
}