using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

/// <summary>
/// Serializable dictionary based on a hybrid of List and Dictionary, offering O(1) cost for regular operations due to increased memory usage (double)
/// </summary>
[Serializable]
public class SDictionary<TKey, TValue> : ISerializationCallbackReceiver, IDictionary<TKey, TValue>
{
    [SerializeField] private List<SKeyValuePair<TKey, TValue>> entries = new List<SKeyValuePair<TKey, TValue>>();

    private Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();

    public int Count => _dictionary.Count;

    /// <summary>
    /// Internal Access
    /// </summary>
    public Dictionary<TKey, TValue> Dictionary => _dictionary;

    public ICollection<TKey> Keys => _dictionary.Keys;

    public ICollection<TValue> Values => _dictionary.Values;

    public bool IsReadOnly => false;

    public void OnBeforeSerialize()
    {
        // Avoid duplicate
        //HashSet<TKey> keysSet = new();
        //List<int> indexToRemove = new();
        //for (int i = 0; i < itemsList.Count; i++)
        //{
        //    if (!keysSet.Add(itemsList[i].key))
        //        indexToRemove.Add(i);
        //}

        //indexToRemove.Reverse();
    }

    public void OnAfterDeserialize()
    {
        _dictionary.Clear();

        foreach (var item in entries)
        {
            if (!_dictionary.ContainsKey(item.key))
                _dictionary.Add(item.key, item.value);
        }
    }

    public void Add(TKey key, TValue value)
    {
        if (!_dictionary.ContainsKey(key))
            entries.Add(new SKeyValuePair<TKey, TValue>(key, value));

        _dictionary.Add(key, value);
    }
    public bool TryGetValue(TKey key, out TValue value) => _dictionary.TryGetValue(key, out value);
    public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);
    public void Clear()
    {
        _dictionary.Clear();
        entries.Clear();
    }

    public bool Remove(TKey key)
    {
        if (_dictionary.ContainsKey(key))
            entries.Remove(new SKeyValuePair<TKey, TValue>(key, _dictionary[key]));

        return _dictionary.Remove(key);
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        if (!_dictionary.ContainsKey(item.Key))
            entries.Add(new SKeyValuePair<TKey, TValue>(item.Key, item.Value));

        _dictionary.Add(item.Key, item.Value);
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return _dictionary.TryGetValue(item.Key, out TValue value) && value.Equals(item.Value);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        if (array == null)
            throw new ArgumentNullException(nameof(array));
        if (arrayIndex < 0 || arrayIndex > array.Length)
            throw new ArgumentOutOfRangeException(nameof(arrayIndex));
        if (array.Length - arrayIndex < Count)
            throw new ArgumentException("Not enough mem space");

        int i = arrayIndex;
        foreach (var kvp in this)
        {
            array[i++] = kvp;
        }
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        if (_dictionary.TryGetValue(item.Key, out TValue value) && value.Equals(item.Value))
        {
            entries.Remove(new SKeyValuePair<TKey, TValue>(item.Key, item.Value));
            return _dictionary.Remove(item.Key); // True
        }
        return false;
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return _dictionary.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _dictionary.GetEnumerator();
    }

    public TValue this[TKey key]
    {
        get => _dictionary[key];
        set
        {
            if (_dictionary.ContainsKey(key))
            {
                _dictionary[key] = value;

                for (int i = 0; i < entries.Count; i++)
                {
                    // Set the same value for all duplicated keys (should not happen ideally)
                    if (EqualityComparer<TKey>.Default.Equals(entries[i].key, key))
                    {
                        entries[i] = new(entries[i].key, value);
                        break;
                    }
                }
            }
            else
            {
                _dictionary[key] = value;
                entries.Add(new(key, value));
            }
        }
    }
}