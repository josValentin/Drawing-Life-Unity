using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.Intrinsics;
using UnityEngine;

public static class GeneralExtensions
{
    public static int[] GetIndices<T>(this List<T> list) => Enumerable.Range(0, list.Count).ToArray();
    public static int[] GetIndices<T>(this T[] arr) => Enumerable.Range(0, arr.Length).ToArray();

    public static int IndexOf<T>(this T[] arr, T value) => System.Array.IndexOf(arr, value);


    public static void ForEach<T>(this IEnumerable<T> enumerable, System.Action<T> action)
    {
        foreach (var item in enumerable) action(item);
    }

    public static void ForEachExcept<T>(this T[] arr, int exceptIndex, System.Action<T> exceptAction, System.Action<T> defaultAction)
    {
        for (int i = 0; i < exceptIndex; i++)
            defaultAction.Invoke(arr[i]);

        exceptAction.Invoke(arr[exceptIndex]);

        for (int i = exceptIndex + 1; i < arr.Length; i++)
            defaultAction.Invoke(arr[i]);
    }

    public static void ForEachExcept<T>(this List<T> list, int exceptIndex, System.Action<T> exceptAction, System.Action<T> defaultAction)
    {
        for (int i = 0; i < exceptIndex; i++)
            defaultAction.Invoke(list[i]);

        exceptAction.Invoke(list[exceptIndex]);

        for (int i = exceptIndex + 1; i < list.Count; i++)
            defaultAction.Invoke(list[i]);
    }

    public static void ForEachChildExcept(this Transform sourceTransform, int exceptIndex, System.Action<Transform> exceptAction, System.Action<Transform> defaultAction)
    {
        for (int i = 0; i < exceptIndex; i++)
            defaultAction?.Invoke(sourceTransform.GetChild(i));

        exceptAction?.Invoke(sourceTransform.GetChild(exceptIndex));

        for (int i = exceptIndex + 1; i < sourceTransform.childCount; i++)
            defaultAction?.Invoke(sourceTransform.GetChild(i));
    }

    public static void ForEachChild(this Transform sourceTransform, System.Action<Transform> action)
    {
        if (action == null)
            return;

        foreach (Transform child in sourceTransform)
        {
            action.Invoke(child);
        }
    }

    public static bool TryFindAnyObjectByType<T>(out T result, FindObjectsInactive includeInactive = FindObjectsInactive.Include) where T : Object
    {
        result = Object.FindAnyObjectByType<T>(includeInactive);
        if (result != null)
        {
            return true;
        }
        return false;
    }

    public static GameObject FindEvenIfInactive(string name)
    {
        var rootGos = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

        // Make a first lookup if any root object is the searched
        foreach (var root in rootGos)
        {
            if (root.name.Equals(name))
                return root;
        }

        // If not, start looking from children
        foreach (var root in rootGos)
        {
            Transform result = TransformExtensions.FindDeep(root.transform, name);
            if (result != null)
                return result.gameObject;
        }
        return null;
    }

    public static void SetActiveGo(this Component component, bool active) => component.gameObject.SetActive(active);
    public static bool IsActiveGo(this Component component) => component.gameObject.activeSelf;
}