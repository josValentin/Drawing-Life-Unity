using UnityEngine;

public static class TransformExtensions
{
    public static void SetLocalPosX(this Transform t, float value) => t.localPosition = new Vector3(value, t.localPosition.y, t.localPosition.z);
    public static void SetLocalPosY(this Transform t, float value) => t.localPosition = new Vector3(t.localPosition.x, value, t.localPosition.z);
    public static void SetLocalPosZ(this Transform t, float value) => t.localPosition = new Vector3(t.localPosition.x, t.localPosition.y, value);

    public static void SetPosX(this Transform t, float value) => t.position = new Vector3(value, t.position.y, t.position.z);
    public static void SetPosY(this Transform t, float value) => t.position = new Vector3(t.position.x, value, t.position.z);
    public static void SetPosZ(this Transform t, float value) => t.position = new Vector3(t.position.x, t.position.y, value);
    public static void SetPos(this Transform t, float x, float y) => t.position = new Vector3(x, y, t.position.z);
    public static void SetPos(this Transform t, float x, float y, float z) => t.position = new Vector3(x, y, z);

    public static void AddPosX(this Transform t, float value) => t.position += Vector3.right * value;
    public static void AddPosY(this Transform t, float value) => t.position += Vector3.up * value;
    public static void AddPosZ(this Transform t, float value) => t.position += Vector3.forward * value;

    public static void SetAnchorPosX(this RectTransform t, float value) => t.anchoredPosition = new Vector3(value, t.anchoredPosition.y);
    public static void SetAnchorPosY(this RectTransform t, float value) => t.anchoredPosition = new Vector3(t.anchoredPosition.x, value);
    public static void SetAnchorPos(this RectTransform t, float x, float y) => t.anchoredPosition = new Vector2(x, y);

    public static void SetSizeDeltaX(this RectTransform t, float value) => t.sizeDelta = new Vector2(value, t.sizeDelta.y);
    public static void SetSizeDeltaY(this RectTransform t, float value) => t.sizeDelta = new Vector2(t.sizeDelta.x, value);
    public static void SetSizeDelta(this RectTransform t, float x, float y) => t.sizeDelta = new Vector2(x, y);

    public static void SetLocalScaleX(this Transform t, float value) => t.localScale = new Vector3(value, t.localScale.y, t.localScale.z);
    public static void SetLocalScaleY(this Transform t, float value) => t.localScale = new Vector3(t.localScale.x, value, t.localScale.z);
    public static void SetLocalScaleZ(this Transform t, float value) => t.localScale = new Vector3(t.localScale.x, t.localScale.y, value);
    public static void SetLocalScale(this Transform t, float x, float y) => t.localScale = new Vector3(x, y, t.localScale.z);
    public static void SetLocalScale(this Transform t, float x, float y, float z) => t.localScale = new Vector3(x, y, z);
    public static void SetLocalScale(this Transform t, float xyz) => t.localScale = Vector3.one * xyz;

    /// <summary>
    /// Returns the transform as a rectTransform component
    /// </summary>
    public static RectTransform AsRect(this Transform t) => t as RectTransform;

    /// <summary>
    /// Search for the child/grandChild that matches the specified name
    /// </summary>
    public static Transform FindDeep(this Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name.Equals(name))
                return child;

            Transform found = FindDeep(child, name);
            if (found != null)
                return found;
        }
        return null;
    }

    /// <summary>
    /// Search for the child/grandChild that matches the specified name
    /// </summary>
    public static T GetComponentInParents<T>(this MonoBehaviour obj, bool includeSelf = true) where T : Component
    {
        if (includeSelf && obj.TryGetComponent<T>(out T selfComp))
            return selfComp;

        Transform parent = obj.transform.parent;
        while (parent != null)
        {
            T comp = parent.GetComponent<T>();
            if (comp != null)
                return comp;
            parent = parent.parent;
        }
        return null;
    }


    public static Transform GetChild(this GameObject go, int index) => go.transform.GetChild(index);
    public static Transform GetChild(this Component co, int index) => co.transform.GetChild(index);
    public static GameObject GetChildGo(this GameObject go, int index) => go.transform.GetChild(index).gameObject;
    public static GameObject GetChildGo(this Component co, int index) => co.transform.GetChild(index).gameObject;
    public static GameObject GetChildGo(this Transform t, int index) => t.GetChild(index).gameObject;
    public static T GetChildComponent<T>(this GameObject go, int index) => go.transform.GetChildComponent<T>(index);
    public static T GetChildComponent<T>(this Component go, int index) => go.transform.GetChildComponent<T>(index);
    public static T GetChildComponent<T>(this Transform t, int index) => t.GetChild(index).GetComponent<T>();
    public static T GetChildComponent<T>(this GameObject go, params int[] indexesPath) => go.transform.GetChildComponent<T>(indexesPath);
    public static T GetChildComponent<T>(this Transform t, params int[] indexesPath)
    {
        Transform target = t;
        foreach (var i in indexesPath)
            target = target.GetChild(i);
        return target.GetComponent<T>();
    }

    public static bool TryGetComponentInChildren<T>(this GameObject go, out T comp, bool includeInactive = true)
    {
        return go.transform.TryGetComponentInChildren<T>(out comp, includeInactive);
    }

    public static bool TryGetComponentInChildren<T>(this Transform t, out T comp, bool includeInactive = true)
    {
        comp = t.GetComponentInChildren<T>(includeInactive);
        return comp != null;
    }

    public static void DestroyChildren(this GameObject go) => go.transform.DestroyChildren();
    public static void DestroyChildren(this Transform t)
    {
        foreach (Transform child in t)
            Object.Destroy(child.gameObject);
    }

    public static int GetChildCount(this GameObject go) => go.transform.childCount;

    public static Transform GetRandomChild(this Transform t) => t.GetChild(GetRandomChildIndex(t));
    public static int GetRandomChildIndex(this Transform t) => Random.Range(0, t.childCount);

    public static Vector2 GetRandomPointInBounds(this Bounds bounds)
    {
        return new Vector2(Random.Range(bounds.min.x, bounds.max.x), Random.Range(bounds.min.y, bounds.max.y));
    }

    public static Bounds WithSizeOffset(this Bounds bounds, float offset)
    {
        return new Bounds(bounds.center, bounds.size + Vector3.one * offset);
    }

    public static bool ContainsX(this Bounds bounds, Vector2 pos)
    {
        return bounds.min.x <= pos.x && pos.x <= bounds.max.x;
    }

    public static bool ContainsY(this Bounds bounds, Vector2 pos)
    {
        return bounds.min.y <= pos.y && pos.y <= bounds.max.y;
    }

    public static void LookAt2D(this Transform source, Vector2 targetPos)
    {
        Vector2 dir = targetPos - (Vector2)source.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        source.rotation = Quaternion.Euler(0, 0, angle);
    }

    public static void SmoothLookAt2D(this Transform source, Vector2 targetPos, float speed)
    {
        Vector2 dir = targetPos - (Vector2)source.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion targetRot = Quaternion.Euler(0, 0, angle);
        source.rotation = Quaternion.Lerp(source.rotation, targetRot, speed * Time.deltaTime);
    }

    public static Vector2 AddAngle(this Vector2 vec, float angle)
    {
        return (Quaternion.Euler(0f, 0f, angle) * vec);
    }
}
