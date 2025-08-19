using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : BaseMonoManager<SoundManager>
{
    [SerializeField]
    private AudioSource[] soloSoundAudioSources;

    [SerializeField]
    private AudioSource[] buttonSoundAudioSources;

    [SerializeField]
    private AudioSourcePool[] audioSourcePools;

    [SerializeField]
    private AudioClip titleBgmClip;

    [SerializeField]
    private AudioListener audioListener;

    private Dictionary<SoundType, string> currentSoloClipPathDic = new()
    {
        { SoundType.Bgm, ""},
        { SoundType.Sfx, ""},
        { SoundType.Amb, ""},
    };

    private string titleBgmPath = PathDefine.BGM_TITLE;
    private bool isFading = false;

    /// <summary>
    /// ratio는 유저세팅 볼륨값에 곱해서 처리함
    /// </summary>
    public async UniTask PlaySoloSound(SoundType soundType, string path)
    {
        var audioSource = GetAudioSourceByType(soundType);

        // 같은 사운드면 재생 X
        if (currentSoloClipPathDic[soundType].Equals(path))
            return;

        currentSoloClipPathDic[soundType] = path;

        if (audioSource.isPlaying)
        {
            if (isFading)
                await UniTaskUtils.WaitWhile(() => isFading);
            else
                await StopFading(audioSource, FloatDefine.SOUND_FADE_TIME);
        }
        
        await LoadAudioClip(audioSource, path);

        if (audioSource.clip != null)
        {
            audioSource.volume = GetVolume(soundType);
            audioSource.Play();
        }

        Logger.Log($"[Sound] Play : {soundType} / {path}");
    }

    public async UniTask PlaySound(SoundType soundType, string path, Vector3 pos = default)
    {
        var audioSourcePool = GetAudioSourcePoolByType(soundType);

        var audioSource = audioSourcePool.GetAudioSource();

        if (audioSource == null)
            return;

        if (pos != Vector3.zero)
        {
            audioSource.spatialBlend = 1;
            audioSource.minDistance = FloatDefine.SOUND_AUDIOSOURCE_MIN_DISTANCE;
            audioSource.maxDistance = FloatDefine.SOUND_AUDIOSOURCE_MAX_DISTANCE;
            audioSource.gameObject.transform.position = pos;
        }
        else
        {
            audioSource.spatialBlend = 0;
            audioSource.gameObject.transform.localPosition = Vector3.zero;
        }

        await LoadAudioClip(audioSource, path);

        if (audioSource.clip != null)
        {
            audioSource.volume = GetVolume(soundType);
            audioSource.Play();
        }
    }

    public async UniTask StopCurrentSoloSound(SoundType soundType)
    {
        var audioSource = GetAudioSourceByType(SoundType.Bgm);

        if (audioSource.isPlaying)
            await StopFading(audioSource, FloatDefine.SOUND_FADE_TIME);

        Logger.Log($"[Sound] Stop : {soundType}");
    }

    public bool IsPlayingSoloSound(SoundType soundType)
    {
        var audioSource = GetAudioSourceByType(soundType);

        return audioSource != null && audioSource.isPlaying;
    }

    public bool IsPlayingSoloSound(SoundType soundType, string path)
    {
        if (!IsPlayingSoloSound(soundType))
            return false;

        if (string.IsNullOrEmpty(currentSoloClipPathDic[soundType]))
            return false;

        if (string.IsNullOrEmpty(path))
            return false;

        return currentSoloClipPathDic[soundType].Equals(path); 
    }

    private AudioSource GetAudioSourceByType(SoundType soundType)
    {
        int index = (int)soundType;

        if (index >= soloSoundAudioSources.Length)
            return null;

        return soloSoundAudioSources[index];
    }

    private AudioSourcePool GetAudioSourcePoolByType(SoundType soundType)
    {
        int index = (int)soundType;

        if (index >= audioSourcePools.Length)
            return null;

        return audioSourcePools[index];
    }

    private async UniTask StopFading(AudioSource source, float duration)
    {
        if (isFading)
            return;

        isFading = true;
        float startVolume = source.volume;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            float progress = elapsedTime / duration;

            source.volume = Mathf.Lerp(startVolume, 0, progress);
            await UniTask.NextFrame(TokenPool.Get(GetHashCode()));
        }

        source.volume = 0;
        source.Stop();
        isFading = false;
    }

    public void PlayButtonSound(ButtonSoundType buttonSoundType)
    {
        int index = (int)buttonSoundType;

        if (index >= buttonSoundAudioSources.Length)
            return;

        var audioSource = buttonSoundAudioSources[index];

        if (audioSource == null)
            return;

        if (audioSource.isPlaying)
            audioSource.Stop();

        audioSource.Play();
    }

    private float GetVolume(SoundType soundType)
    {
        var masterVolume = UserSettings.Volume.Master;

        float volume = UserSettings.GetVolume(soundType);

        return volume * masterVolume;
    }

    private async UniTask LoadAudioClip(AudioSource source, string path)
    {
        if (source == null)
            return;

        if (path.Equals(titleBgmPath))
        {
            source.clip = titleBgmClip;
            return;
        }

        await AddressableManager.Instance.SafeLoadAsync(source, path);
    }
}