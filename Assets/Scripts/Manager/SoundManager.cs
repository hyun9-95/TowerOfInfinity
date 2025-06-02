using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : BaseMonoManager<SoundManager>
{
    [SerializeField]
    private AudioSource[] audioSources;

    [SerializeField]
    private AudioListener audioListener;

    /// <summary>
    /// ratio는 유저세팅 볼륨값에 곱해서 처리함
    /// </summary>
    public async UniTask PlaySound(string name, SoundType soundType, float volumeRatio = 1f)
    {
        int index = (int)soundType;

        if (index >= audioSources.Length)
            return;

        var audioSource = audioSources[index];

        if (audioSource == null)
            return;

        await LoadAudioClip(audioSource, name);

        if (audioSource.clip != null)
        {
            audioSource.volume = GetVolume(soundType, volumeRatio);
            audioSource.Play();
        }
    }

    private float GetVolume(SoundType soundType, float volumeRatio)
    {
        if (volumeRatio > 1)
            volumeRatio = 1;

        float volume = GameManager.Instance.Settings.GetVolume(soundType);

        return volume * volumeRatio;
    }

    private async UniTask LoadAudioClip(AudioSource source, string path)
    {
        if (source == null)
            return;

        await AddressableManager.Instance.SafeLoadAsync(source, path);
    }
}