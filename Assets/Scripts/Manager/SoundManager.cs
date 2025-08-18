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
        if (isFading)
            await UniTask.WaitWhile(() => isFading);

        int index = (int)soundType;

        if (index >= soloSoundAudioSources.Length)
            return;

        var audioSource = soloSoundAudioSources[index];

        if (audioSource == null)
            return;

        // 같은 사운드면 재생 X
        if (currentSoloClipPathDic[soundType].Equals(path))
            return;

        if (audioSource.isPlaying)
            await StopFading(audioSource, 1f);

        await LoadAudioClip(audioSource, path);

        if (audioSource.clip != null)
        {
            audioSource.volume = GetVolume(soundType);
            audioSource.Play();
            currentSoloClipPathDic[soundType] = path;
        }
    }

    public async UniTask StopCurrentSoloSound(SoundType soundType)
    {
        int index = (int)soundType;

        if (index >= soloSoundAudioSources.Length)
            return;

        var audioSource = soloSoundAudioSources[index];

        if (audioSource == null || !audioSource.isPlaying)
            return;

        await StopFading(audioSource, FloatDefine.SOUND_FADE_TIME);
    }

    private async UniTask StopFading(AudioSource source, float duration)
    {
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

        audioSource.volume = GetVolume(SoundType.Sfx);
        audioSource.Play();
    }

    private float GetVolume(SoundType soundType, float volumeRatio = 1)
    {
        if (volumeRatio > 1)
            volumeRatio = 1;

        if (PlayerManager.Instance.UserSettings == null)
            return 1;

        float volume = PlayerManager.Instance.UserSettings == null ?
            1 : PlayerManager.Instance.UserSettings.GetVolume(soundType);

        return volume * volumeRatio;
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