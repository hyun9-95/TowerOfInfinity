#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    private Dictionary<GameObject, Dictionary<UnityEngine.Object, AsyncOperationHandle>> trackingAssetHandles = new Dictionary<GameObject, Dictionary<UnityEngine.Object, AsyncOperationHandle>>();
    private HashSet<string> loadedScenes = new HashSet<string>();
    private HashSet<GameObject> excludedFromRelease = new HashSet<GameObject>();

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
        string address = define.ToString();

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
        string address = define.ToString();

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

    private async UniTask<T> LoadAssetAsync<T>(string address, bool isHandle = true) where T : UnityEngine.Object
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
    /// Tracker가 파괴되었으면 ReleaseGameObject 호출 필요
    /// </summary>
    public async UniTask<T> LoadAssetAsyncWithTracker<T>(string address, GameObject tracker) where T : UnityEngine.Object
    {
        if (!IsContain(address))
            return null;

        if (tracker == null)
        {
            Logger.Error("Tracker를 등록해야 로드가 가능!");
            return null;
        }

        var handle = Addressables.LoadAssetAsync<T>(address);
        var asset = await handle;

        if (asset == null)
        {
            Addressables.Release(handle);
            return null;
        }

        if (tracker != null)
        {
            if (trackingAssetHandles.TryGetValue(tracker, out var assetToHandleMap))
            {
                assetToHandleMap[asset] = handle;
            }
            else
            {
                trackingAssetHandles.Add(tracker, new Dictionary<UnityEngine.Object, AsyncOperationHandle>() { { asset, handle } });
            }
        }

        return asset;
    }

    /// <summary>
    /// 참조가 해제되지 않게 유지하려면 isHandle을 false로..
    /// </summary>
    public async UniTask<T> LoadScriptableObject<T>(string address, bool isHandle = true) where T : ScriptableObject
    {
        return await LoadAssetAsync<T>(address, isHandle);
    }

    public async UniTask<GameObject> InstantiateAsync(string address, Transform transform = null)
    {
        if (!IsContain(address))
            return null;

        var handle = Addressables.InstantiateAsync(address, transform);
        var go = await handle;

        if (go != null)
        {
            instantiatedHandles[go] = handle;
        }
        else
        {
            Logger.Error($"Instantiate Failed : {address}");
        }

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

    /// <summary>
    /// 참조가 계속 유지되어야 하는 것들만 이것으로 로드하자.
    /// </summary>
    public async UniTask<T> InstantiateUntrackedAsync<T>(string address, Transform transform = null) where T : MonoBehaviour
    {
        if (!IsContain(address))
            return null;

        var handle = Addressables.InstantiateAsync(address, transform);
        var go = await handle;

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

        if (trackingAssetHandles.TryGetValue(go, out var assetToHandleMap))
        {
            foreach (var kvp in assetToHandleMap)
            {
                var trackingHandle = kvp.Value;

                if (trackingHandle.IsValid())
                    Addressables.Release(trackingHandle);
            }
            
            trackingAssetHandles.Remove(go);
        }
    }

    private void ReleaseAsset(UnityEngine.Object asset)
    {
        if (asset == null)
            return;

        if (assetHandles.TryGetValue(asset, out var handle))
        {
            Addressables.Release(handle);
            assetHandles.Remove(asset);
        }
    }

    public void ReleaseFromTracker(Object asset, GameObject tracker)
    {
        if (trackingAssetHandles.TryGetValue(tracker, out var assetToHandleMap))
        {
            if (assetToHandleMap.TryGetValue(asset, out var handle))
            {
                if (handle.IsValid())
                    Addressables.Release(handle);
                
                assetToHandleMap.Remove(asset);
                
                if (assetToHandleMap.Count == 0)
                    trackingAssetHandles.Remove(tracker);
            }
        }
    }

    public void ReleaseAllHandles()
    {
        var goArray = instantiatedHandles.Keys.ToArray();

        foreach (var go in goArray)
            ReleaseGameObject(go);

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

        if (cachedSprite != null && cachedSprite != image.sprite)
            ReleaseAsset(cachedSprite);
    }

    public async UniTask SafeLoadAsync(AudioSource audioSource, string path)
    {
        var cachedClip = audioSource.clip;
        audioSource.clip = await LoadAssetAsync<AudioClip>(path);

        if (cachedClip != null && cachedClip != audioSource.clip)
            ReleaseAsset(cachedClip);
    }

    public async UniTask SafeLoadAsync(SpriteRenderer spriteRenderer, string path)
    {
        var cachedSprite = spriteRenderer.sprite;
        spriteRenderer.sprite = await LoadAssetAsync<Sprite>(path);

        if (cachedSprite != null && cachedSprite != spriteRenderer.sprite)
            ReleaseAsset(cachedSprite);
    }

    public async UniTask SafeLoadAsync(RawImage rawImage, string path)
    {
        var cachedTexture = rawImage.texture;
        rawImage.texture = await LoadAssetAsync<Texture>(path);

        if (cachedTexture != null && cachedTexture != rawImage.texture)
            ReleaseAsset(cachedTexture);
    }

    public async UniTask SafeLoadAsync(VideoPlayer videoPlayer, string path)
    {
        var cachedClip = videoPlayer.clip;
        videoPlayer.clip = await LoadAssetAsync<VideoClip>(path);

        if (cachedClip != null && cachedClip != videoPlayer.clip)
            ReleaseAsset(cachedClip);
    }

    public async UniTask SafeLoadAsync(Animator animator, string path)
    {
        var cachedController = animator.runtimeAnimatorController;
        animator.runtimeAnimatorController = await LoadAssetAsync<RuntimeAnimatorController>(path);

        if (cachedController != null && cachedController != animator.runtimeAnimatorController)
            ReleaseAsset(cachedController);
    }
    #endregion
}