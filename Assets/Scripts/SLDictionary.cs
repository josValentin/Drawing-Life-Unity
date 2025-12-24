using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Serializable dictionary based entirely on List, providing optimal memory usage at a cost of O(n) for regular operations
/// </summary>
[System.Serializable]
public class SLDictionary<TKey, TValue> : IDictionary<TKey, TValue>
{
    [SerializeField] private List<SKeyValuePair<TKey, TValue>> entries = new List<SKeyValuePair<TKey, TValue>>();

    /// <summary>
    /// Get Dictionary from entries
    /// </summary>
    public Dictionary<TKey, TValue> Dictionary => entries.ToDictionary(p => p.key, p => p.value);

    public ICollection<TKey> Keys => entries.Select(p => p.key).ToList();

    public ICollection<TValue> Values => entries.Select(p => p.value).ToList();

    public int Count => entries.Count;

    public bool IsReadOnly => false;

    public void Add(TKey key, TValue value) => entries.Add(new(key, value));
    public bool TryGetValue(TKey key, out TValue value)
    {
        int index = GetIndex(key);

        if (index == -1)
        {
            value = default;
            return false;
        }

        value = entries[index].value;
        return true;
    }
    public bool ContainsKey(TKey key) => GetIndex(key) != -1;

    private int GetIndex(TKey key) => entries.FindIndex(p => p.key.Equals(key));

    public void Clear()
    {
        entries.Clear();
    }

    public bool Remove(TKey key)
    {
        int index = entries.FindIndex(e => e.key.Equals(key));
        if (index > -1)
        {
            entries.RemoveAt(index);
            return true;
        }
        return false;
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        entries.Add(SKeyValuePair.From(item));
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return entries.Contains(SKeyValuePair.From(item));
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
        return entries.Remove(SKeyValuePair.From(item));
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return Dictionary.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Dictionary.GetEnumerator();
    }

    public TValue this[TKey key]
    {
        get
        {
            if (TryGetValue(key, out TValue value))
            {
                return value;
            }
            return default;
        }
        set
        {
            int index = GetIndex(key);
            if (index != -1)
            {
                entries[index] = new(key, value);

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
                entries.Add(new(key, value));
            }
        }
    }
}
