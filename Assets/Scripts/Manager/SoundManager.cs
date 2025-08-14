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
    private AudioListener audioListener;

    private Dictionary<SoundType, string> currentSoloClipPathDic = new()
    {
        { SoundType.Bgm, ""},
        { SoundType.Sfx, ""},
    };

    /// <summary>
    /// ratio는 유저세팅 볼륨값에 곱해서 처리함
    /// </summary>
    public async UniTask PlaySoloSound(string path, SoundType soundType)
    {
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
            audioSource.Stop();

        await LoadAudioClip(audioSource, path);

        if (audioSource.clip != null)
        {
            audioSource.volume = GetVolume(soundType, 1);
            audioSource.Play();
            currentSoloClipPathDic[soundType] = path;
        }
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

        audioSource.volume = GetVolume(SoundType.Sfx, 1);
        audioSource.Play();
    }

    private float GetVolume(SoundType soundType, float volumeRatio)
    {
        if (volumeRatio > 1)
            volumeRatio = 1;

        float volume = PlayerManager.instance.UserSettings == null ?
            1 : PlayerManager.Instance.UserSettings.GetVolume(soundType);

        return volume * volumeRatio;
    }

    private async UniTask LoadAudioClip(AudioSource source, string path)
    {
        if (source == null)
            return;

        await AddressableManager.Instance.SafeLoadAsync(source, path);
    }
}