using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Linq;
using System.Text;
using UnityEngine;

public static class ExtensionUtils
{
    public static bool IsValidArray<T>(this T[] array)
    {
        return array != null && array.Length > 0;
    }

    public static bool IntegrityCheck(this byte[] bytes, byte[] compareBytes)
    {
        if (!bytes.IsValidArray() || !compareBytes.IsValidArray())
            return false;

        using (System.Security.Cryptography.MD5 md5Hash1 = System.Security.Cryptography.MD5.Create())
        using (System.Security.Cryptography.MD5 md5Hash2 = System.Security.Cryptography.MD5.Create())
        {
            return md5Hash1.ComputeHash(bytes).SequenceEqual(md5Hash2.ComputeHash(compareBytes));
        }
    }

    public static byte[] GetSHA256(this byte[] bytes)
    {
        if (!bytes.IsValidArray())
            return null;

        using (System.Security.Cryptography.SHA256 sha256 = System.Security.Cryptography.SHA256.Create())
        {
            return sha256.ComputeHash(bytes);
        }
    }

    public static string GetStringUTF8(this byte[] bytes)
    {
        try
        {
            if (!bytes.IsValidArray())
                return null;

            return Encoding.UTF8.GetString(bytes);
        }
        catch (Exception e)
        {
            Logger.Exception("Failed to UTF8 Encoding", e);
            return null;
        }
    }

    public static void SafeSetText(this TMPro.TextMeshProUGUI text, string value)
    {
        if (CheckSafeNull(text))
            return;

        if (text != null)
            text.text = value;
    }

    public static void SafeSetActive(this UnityEngine.GameObject gameObject, bool active)
    {
        if (CheckSafeNull(gameObject))
            return;

        if (gameObject.activeSelf == active)
            return;

        gameObject.SetActive(active);
    }

    public static void SortByNearest(this Collider2D[] colliders, Vector3 center)
    {
        if (colliders == null)
            return;

        Array.Sort(colliders, (a, b) =>
        {
            float distanceA = (a.transform.position - center).sqrMagnitude;
            float distanceB = (b.transform.position - center).sqrMagnitude;

            return distanceA.CompareTo(distanceB);
        });
    }

    public static bool CheckSafeNull(this UnityEngine.Object obj)
    {
        return !obj;
    }

    public static GameObject GetRootObject(this UnityEngine.SceneManagement.Scene scene)
    {
        var rootGameObjects = scene.GetRootGameObjects();

        if (rootGameObjects.Length == 0)
            return null;

        return rootGameObjects[0];
    }

    public static T GetRootComponent<T>(this UnityEngine.SceneManagement.Scene scene) where T : Component
    {
        var rootGameObject = scene.GetRootObject();

        if (!rootGameObject)
            return null;

        return rootGameObject.GetComponent<T>();
    }

    public static bool CheckLayer(this GameObject gameObject, LayerFlag layer)
    {
        return (layer & (LayerFlag)(1 << gameObject.layer)) != 0;
    }

    public static bool CheckLayer(this GameObject gameObject, LayerInt layer)
    {
        return (int)layer == gameObject.layer;
    }

    public static bool IsFixedType(this DirectionType directionType)
    {
        return directionType switch
        {
            _ => true
        };
    }

    public static void FadeOff(this SpriteRenderer renderer, float duration, GameObject gameObject, Action callback = null)
    {
        if (duration == 0)
        {
            SetAlpha(renderer, 0);
            return;
        }

        renderer.DOFade(0f, duration).SetEase(Ease.Linear).OnComplete(() =>
        {
            gameObject.SafeSetActive(false);
            callback?.Invoke(); 
        });
    }

    public static void RestoreAlpha(this SpriteRenderer renderer, float alpha = 1f)
    {
        SetAlpha(renderer, alpha);
    }

    public static void SetAlpha(this SpriteRenderer renderer, float alpha)
    {
        var color = renderer.color;
        color.a = alpha;
        renderer.color = color;
    }

    public static void FollowWorldPosition(this RectTransform rectTransform, Vector3 pos, Camera worldCamera, RectTransform parentRect, Vector2 offset)
    {
        Vector3 screenPos = worldCamera.WorldToScreenPoint(pos);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            screenPos,
            worldCamera,
            out Vector2 localPoint
        );

        rectTransform.anchoredPosition = localPoint + offset;
    }

#if UNITY_EDITOR
    public static void InitializeWindow(this UnityEditor.EditorWindow window, float width, float height)
    {
        window.minSize = new Vector2(width, height);
        window.maxSize = new Vector2(width * 2, height);
    }
#endif
}
