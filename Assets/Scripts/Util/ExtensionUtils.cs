using DG.Tweening;
using System;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Cysharp.Threading.Tasks;

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
        if (text.CheckSafeNull())
            return;

        if (text != null)
            text.text = value;
    }

    public static void SafeSetActive(this UnityEngine.GameObject gameObject, bool active)
    {
        if (gameObject.CheckSafeNull())
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
        return !obj || obj.Equals(null);
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

    public static void DeactiveWithFade(this SpriteRenderer renderer, float duration, GameObject gameObject, Action callback = null)
    {
        renderer.DOFade(0f, duration).SetEase(Ease.Linear).OnComplete(() =>
        {
            gameObject.SafeSetActive(false);
            callback?.Invoke(); 
        });
    }

    public static void FadeIn(this SpriteRenderer renderer, float duration, Action callback = null)
    {
        renderer.DOFade(1f, duration).SetEase(Ease.Linear).OnComplete(() =>
        {
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

    public static Vector2 RotateVector2(this Vector2 vector, float angleDegrees)
    {
        float angleRad = angleDegrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(angleRad);
        float sin = Mathf.Sin(angleRad);

        return new Vector2(
            vector.x * cos - vector.y * sin,
            vector.x * sin + vector.y * cos
        );
    }

    public static TeamTag Opposite(this TeamTag tag)
    {
        if (tag == TeamTag.Ally)
            return TeamTag.Enemy;

        return TeamTag.Ally;
    }

    #region Addressable SafeLoad Extensions
    public static async UniTask SafeLoadAsync(this Image image, string path)
    {
        await AddressableManager.Instance.SafeLoadAsync(image, path);
    }

    public static async UniTask SafeLoadAsync(this AudioSource audioSource, string path)
    {
        await AddressableManager.Instance.SafeLoadAsync(audioSource, path);
    }

    public static async UniTask SafeLoadAsync(this SpriteRenderer spriteRenderer, string path)
    {
        await AddressableManager.Instance.SafeLoadAsync(spriteRenderer, path);
    }

    public static async UniTask SafeLoadAsync(this RawImage rawImage, string path)
    {
        await AddressableManager.Instance.SafeLoadAsync(rawImage, path);
    }

    public static async UniTask SafeLoadAsync(this VideoPlayer videoPlayer, string path)
    {
        await AddressableManager.Instance.SafeLoadAsync(videoPlayer, path);
    }

    public static async UniTask SafeLoadAsync(this Animator animator, string path)
    {
        await AddressableManager.Instance.SafeLoadAsync(animator, path);
    }
    #endregion

#if UNITY_EDITOR
    public static void InitializeWindow(this UnityEditor.EditorWindow window, float width, float height)
    {
        window.minSize = new Vector2(width, height);
        window.maxSize = new Vector2(width * 2, height);
    }
#endif
}
