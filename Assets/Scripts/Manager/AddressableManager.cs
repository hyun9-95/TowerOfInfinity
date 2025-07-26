#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class AddressableManager : BaseManager<AddressableManager>
{
    public enum Sequence
    {
        LoadAddressableVersion,
        LoadAddressableBuildInfo,
        LoadAddressableBuild,
        Done,
    }

    public Sequence CurrentSequence { get; private set; } = Sequence.LoadAddressableVersion;

    private AddressableBuildInfo addressableBuildInfo;
    private bool clearBundle;

    private Dictionary<GameObject, AsyncOperationHandle<GameObject>> instantiatedHandles = new Dictionary<GameObject, AsyncOperationHandle<GameObject>>();
    private Dictionary<UnityEngine.Object, AsyncOperationHandle> assetHandles = new Dictionary<UnityEngine.Object, AsyncOperationHandle>();
    private Dictionary<string, AsyncOperationHandle<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance>> sceneHandles = new Dictionary<string, AsyncOperationHandle<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance>>();
    private HashSet<string> loadedScenes = new HashSet<string>();

    #region Initialize Addressable
    /// <summary>
    /// FireBase 나중에 바꾸는거 고려해야함.. 비용문제
    /// </summary>
    /// <returns></returns>
    public void LoadLocalAddressableBuildAsync()
    {
#if UNITY_EDITOR
        addressableBuildInfo = GenerateAddressableBuildInfo(PathDefine.Addressable);
#endif
    }

#if UNITY_EDITOR
    private AddressableBuildInfo GenerateAddressableBuildInfo(string addressableAssetPath)
    {
        Dictionary<string, string> buildInfoDic = new Dictionary<string, string>();
        string[] guids = UnityEditor.AssetDatabase.FindAssets("t:Object", new[] { addressableAssetPath });

        foreach (var guid in guids)
        {
            string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);

            // 폴더 제외
            if (UnityEditor.AssetDatabase.IsValidFolder(assetPath))
                continue;

            string address = assetPath.Replace("Assets/Addressable/", "");
            string[] split = address.Split(".");
            address = split[0];

            if (!buildInfoDic.ContainsKey(address))
            {
                buildInfoDic.Add(address, assetPath);
            }
            else
            {
                Logger.Warning($"중복된 주소: {address}");
            }
        }

        return new AddressableBuildInfo(null, buildInfoDic);
    }
#endif

    private bool IsContain(string address)
    {
        if (string.IsNullOrEmpty(address))
        {
            Logger.Null($"Addressable Name {address}");
            return false;
        }

        return addressableBuildInfo.AddressableDic.ContainsKey(address);
    }

    private string GetSceneAdress(SceneDefine define)
    {
        return $"Scenes/{define}/{define}";
    }

    private bool IsAddressableScene(SceneDefine define)
    {
        return define switch
        {
            SceneDefine.RootScene => false,
            _ => true,
        };
    }
    #endregion

    public async UniTask<Scene> LoadSceneAsync(SceneDefine define,
    UnityEngine.SceneManagement.LoadSceneMode loadSceneMode = UnityEngine.SceneManagement.LoadSceneMode.Single)
    {
        string address = GetSceneAdress(define);

        if (IsAddressableScene(define))
        {
            var handle = Addressables.LoadSceneAsync(address, loadSceneMode);
            var sceneInstance = await handle;

            if (sceneInstance.Scene == null)
                return default;

            if (sceneInstance.Scene.isLoaded)
            {
                sceneHandles[address] = handle;
                loadedScenes.Add(address);
                return sceneInstance.Scene;
            }
            else
            {
                Addressables.Release(handle);
                return default;
            }
        }
        else
        {
            loadedScenes.Add(address);
            await UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(address, loadSceneMode);
        }
        return default;
    }

    public async UniTask<bool> UnloadSceneAsync(SceneDefine define)
    {
        string address = GetSceneAdress(define);

        if (!loadedScenes.Contains(address))
        {
            Logger.Warning($"Scene not loaded: {address}");
            return false;
        }

        if (IsAddressableScene(define) && sceneHandles.TryGetValue(address, out var handle))
        {
            // Addressable로 로드된 씬 언로드
            await Addressables.UnloadSceneAsync(handle);
            sceneHandles.Remove(address);
        }
        else
        {
            // Resources로 로드된 씬 언로드
            await UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(address);
        }

        await Resources.UnloadUnusedAssets();

        loadedScenes.Remove(address);
        return true;
    }

    public async UniTask<T> LoadAssetAsync<T>(string address, bool isHandle = true) where T : UnityEngine.Object
    {
        if (!IsContain(address))
            return null;

        var handle = Addressables.LoadAssetAsync<T>(address);
        var asset = await handle;

        if (asset == null)
        {
            Addressables.Release(handle);
            return null;
        }

        if (isHandle)
            assetHandles[asset] = handle;

        return asset;
    }

    /// <summary>
    /// 참조가 해제되지 않게 유지하려면 isHandle을 false로..
    /// </summary>
    public async UniTask<T> LoadScriptableObject<T>(string address, bool isHandle = true) where T : ScriptableObject
    {
        return await LoadAssetAsync<T>(address, isHandle);
    }

    /// <summary>
    /// ObjectPoolManager 외에서 호출하면 안된다.
    /// </summary>
    public async UniTask<GameObject> InstantiateAsync(string address, Transform transform = null)
    {
        if (!IsContain(address))
            return null;

        var handle = Addressables.InstantiateAsync(address, transform);
        var go = await handle;

        if (go != null)
            instantiatedHandles[go] = handle;

        return go;
    }

    public async UniTask<T> InstantiateAddressableMonoAsync<T>(string address, Transform transform = null)
        where T : AddressableMono
    {
        if (!IsContain(address))
            return null;

        GameObject go = await InstantiateAsync(address, transform);

        if (go == null)
            return null;

        return go.GetComponent<T>();
    }

    public void ReleaseGameObject(GameObject go)
    {
        if (go == null)
            return;

        if (instantiatedHandles.TryGetValue(go, out var handle))
        {
            Addressables.ReleaseInstance(handle);
            instantiatedHandles.Remove(go);
        }
    }

    public void ReleaseAsset(UnityEngine.Object asset)
    {
        if (asset == null)
            return;

        if (assetHandles.TryGetValue(asset, out var handle))
        {
            Addressables.Release(handle);
            assetHandles.Remove(asset);
        }
    }

    public void ReleaseAllHandles()
    {
        foreach (var handle in instantiatedHandles.Values)
        {
            if (handle.IsValid())
                Addressables.ReleaseInstance(handle);
        }

        instantiatedHandles.Clear();

        foreach (var handle in assetHandles.Values)
        {
            if (handle.IsValid())
                Addressables.Release(handle);
        }

        assetHandles.Clear();
    }

    public void ReleaseAllSceneHandles()
    {
        foreach (var handle in sceneHandles.Values)
        {
            if (handle.IsValid())
                Addressables.UnloadSceneAsync(handle).ToUniTask().Forget();
        }

        sceneHandles.Clear();
        loadedScenes.Clear();
    }

    public bool IsSceneLoaded(SceneDefine sceneName)
    {
        return loadedScenes.Contains(sceneName.ToString());
    }

    #region SafeLoad
    public async UniTask SafeLoadAsync(Image image, string path)
    {
        var cachedSprite = image.sprite;
        image.sprite = await LoadAssetAsync<Sprite>(path);

        if (cachedSprite != null)
            ReleaseAsset(cachedSprite);
    }

    public async UniTask SafeLoadAsync(AudioSource audioSource, string path)
    {
        var cachedClip = audioSource.clip;
        audioSource.clip = await LoadAssetAsync<AudioClip>(path);

        if (cachedClip != null)
            ReleaseAsset(cachedClip);
    }

    public async UniTask SafeLoadAsync(SpriteRenderer spriteRenderer, string path)
    {
        var cachedSprite = spriteRenderer.sprite;
        spriteRenderer.sprite = await LoadAssetAsync<Sprite>(path);

        if (cachedSprite != null)
            ReleaseAsset(cachedSprite);
    }

    public async UniTask SafeLoadAsync(RawImage rawImage, string path)
    {
        var cachedTexture = rawImage.texture;
        rawImage.texture = await LoadAssetAsync<Texture>(path);

        if (cachedTexture != null)
            ReleaseAsset(cachedTexture);
    }

    public async UniTask SafeLoadAsync(VideoPlayer videoPlayer, string path)
    {
        var cachedClip = videoPlayer.clip;
        videoPlayer.clip = await LoadAssetAsync<VideoClip>(path);

        if (cachedClip != null)
            ReleaseAsset(cachedClip);
    }

    public async UniTask SafeLoadAsync(Animator animator, string path)
    {
        var cachedController = animator.runtimeAnimatorController;
        animator.runtimeAnimatorController = await LoadAssetAsync<RuntimeAnimatorController>(path);

        if (cachedController != null)
            ReleaseAsset(cachedController);
    }
    #endregion
}