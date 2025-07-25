#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
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
    private Dictionary<TrackableMono, List<AsyncOperationHandle>> addressableMonoHandles = new Dictionary<TrackableMono, List<AsyncOperationHandle>>();
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

            string assetName = Path.GetFileNameWithoutExtension(assetPath);

            if (!buildInfoDic.ContainsKey(assetName))
            {
                buildInfoDic.Add(assetName, assetPath);
            }
            else
            {
                Logger.Warning($"중복된 이름: {assetName}");
            }
        }

        return new AddressableBuildInfo(null, buildInfoDic);
    }
#endif
    #endregion

    private bool IsContain(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            Logger.Null($"Addressable Name {name}");
            return false;
        }

        return addressableBuildInfo.AddressableDic.ContainsKey(name);
    }

    public async UniTask<Scene> LoadSceneAsyncWithName(SceneDefine define,
    UnityEngine.SceneManagement.LoadSceneMode loadSceneMode = UnityEngine.SceneManagement.LoadSceneMode.Single)
    {
        string name = define.ToString();

        if (IsContain(name))
        {
            var handle = Addressables.LoadSceneAsync(name, loadSceneMode);
            var sceneInstance = await handle;

            if (sceneInstance.Scene == null)
                return default;

            if (sceneInstance.Scene.isLoaded)
            {
                sceneHandles[name] = handle;
                loadedScenes.Add(name);
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
            loadedScenes.Add(name);
            await UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(name, loadSceneMode);
        }
        return default;
    }

    public async UniTask<bool> UnloadSceneAsync(SceneDefine sceneName)
    {
        string name = sceneName.ToString();

        if (!loadedScenes.Contains(name))
        {
            Logger.Warning($"Scene not loaded: {name}");
            return false;
        }

        if (IsContain(name) && sceneHandles.TryGetValue(name, out var handle))
        {
            // Addressable로 로드된 씬 언로드
            await Addressables.UnloadSceneAsync(handle);
            sceneHandles.Remove(name);
        }
        else
        {
            // Resources로 로드된 씬 언로드
            await UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(name);
        }

        await Resources.UnloadUnusedAssets();

        loadedScenes.Remove(name);
        return true;
    }

    /// <summary>
    /// 사용 후 꼭 참조해제하기!!!!
    /// </summary>
    private async UniTask<T> LoadAssetAsyncWithName<T>(string assetName, TrackableMono tracker, bool isHandle = true) where T : UnityEngine.Object
    {
        if (!IsContain(assetName))
            return null;

        var handle = Addressables.LoadAssetAsync<T>(assetName);
        var asset = await handle;

        if (asset == null)
        {
            Addressables.Release(handle);
            return null;
        }

        if (isHandle)
        {
            if (tracker != null)
            {
                if (addressableMonoHandles.TryGetValue(tracker, out var assetHandleList))
                {
                    assetHandleList.Add(handle);
                }
                else
                {
                    var newAssetHandles = new List<AsyncOperationHandle>();
                    newAssetHandles.Add(handle);
                    addressableMonoHandles[tracker] = newAssetHandles;
                }
            }

            assetHandles[asset] = handle;
        }

        return asset;
    }

    /// <summary>
    /// Tracker를 등록하지 않으면 Flow 변경시에 해제된다.
    /// </summary>
    public async UniTask<T> LoadScriptableObject<T>(string infoName, TrackableMono tracker = null) where T : ScriptableObject
    {
        return await LoadAssetAsyncWithName<T>(infoName, tracker, true);
    }

    /// <summary>
    /// 해제되지 않아야하는 ScriptableObject를 불러올 때 사용한다.
    /// </summary>
    public async UniTask<T> LoadStaticScriptableObject<T>(string infoName) where T : ScriptableObject
    {
        return await LoadAssetAsyncWithName<T>(infoName, null, false);
    }

    /// <summary>
    /// ObjectPoolManager 외에서 호출하면 안된다.
    /// </summary>
    public async UniTask<GameObject> InstantiateAsyncWithName(string name, Transform transform = null)
    {
        if (!IsContain(name))
            return null;

        var handle = Addressables.InstantiateAsync(name, transform);
        var go = await handle;

        if (go != null)
            instantiatedHandles[go] = handle;

        return go;
    }

    public async UniTask<T> InstantiateAddressableMonoAsync<T>(string name, Transform transform = null)
        where T : AddressableMono
    {
        if (!IsContain(name))
            return null;

        GameObject go = await InstantiateAsyncWithName(name, transform);

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
            if (handle.IsValid())
                Addressables.ReleaseInstance(handle);

            instantiatedHandles.Remove(go);
        }
    }

    public void ReleaseTrackerAssets(TrackableMono tracker)
    {
        if (tracker == null)
            return;

        if (addressableMonoHandles.TryGetValue(tracker, out var handles))
        {
            foreach (var handle in handles)
            {
                if (handle.IsValid())
                    Addressables.Release(handle);
            }

            addressableMonoHandles.Remove(tracker);
        }
    }

    public void ReleaseAsset(UnityEngine.Object asset)
    {
        if (asset == null)
            return;

        if (assetHandles.TryGetValue(asset, out var handle))
        {
            if (handle.IsValid())
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
    public async UniTask SafeLoadAsync(Image image, string path, TrackableMono tracker)
    {
        var cachedSprite = image.sprite;
        image.sprite = await LoadAssetAsyncWithName<Sprite>(path, tracker);

        if (cachedSprite != null)
            ReleaseAsset(cachedSprite);
    }

    public async UniTask SafeLoadAsync(AudioSource audioSource, string path, TrackableMono tracker)
    {
        var cachedClip = audioSource.clip;
        audioSource.clip = await LoadAssetAsyncWithName<AudioClip>(path, tracker);

        if (cachedClip != null)
            ReleaseAsset(cachedClip);
    }

    public async UniTask SafeLoadAsync(SpriteRenderer spriteRenderer, string path, TrackableMono tracker)
    {
        var cachedSprite = spriteRenderer.sprite;
        spriteRenderer.sprite = await LoadAssetAsyncWithName<Sprite>(path, tracker);

        if (cachedSprite != null)
            ReleaseAsset(cachedSprite);
    }

    public async UniTask SafeLoadAsync(RawImage rawImage, string path, TrackableMono tracker)
    {
        var cachedTexture = rawImage.texture;
        rawImage.texture = await LoadAssetAsyncWithName<Texture>(path, tracker);

        if (cachedTexture != null)
            ReleaseAsset(cachedTexture);
    }

    public async UniTask SafeLoadAsync(VideoPlayer videoPlayer, string path, TrackableMono tracker)
    {
        var cachedClip = videoPlayer.clip;
        videoPlayer.clip = await LoadAssetAsyncWithName<VideoClip>(path, tracker);

        if (cachedClip != null)
            ReleaseAsset(cachedClip);
    }

    public async UniTask SafeLoadAsync(Animator animator, string path, TrackableMono tracker)
    {
        var cachedController = animator.runtimeAnimatorController;
        animator.runtimeAnimatorController = await LoadAssetAsyncWithName<RuntimeAnimatorController>(path, tracker);

        if (cachedController != null)
            ReleaseAsset(cachedController);
    }
    #endregion
}