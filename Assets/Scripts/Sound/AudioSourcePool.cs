using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourcePool : MonoBehaviour
{
    private Queue<AudioSource> audioSourceQueue;
    private float volume = 1.0f;

    public void SetVolume(float value)
    {
        volume = value;
    }

    public void Clear()
    {
        if (audioSourceQueue != null)
        {
            while (audioSourceQueue.Count > 0)
            {
                var audioSource = audioSourceQueue.Dequeue();
                Destroy(audioSource.gameObject);
            }

            audioSourceQueue.Clear();
        }
    }

    public AudioSource GetAudioSource()
    {
        if (audioSourceQueue == null)
            audioSourceQueue = new Queue<AudioSource>();

        if (audioSourceQueue.Count > 0)
        {
            var audioSource = audioSourceQueue.Dequeue();
            audioSource.volume = volume;
            return audioSource;
        }

        return CreateNewAudioSorce();
    }

    private AudioSource CreateNewAudioSorce()
    {
        GameObject audioSourceObject = new GameObject("AudioSource");
        audioSourceObject.transform.SetParent(this.transform);

        AudioSource audioSource = audioSourceObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.volume = volume;

        return audioSource;
    }

    public async UniTask AutoReturnToPool(AudioSource audioSource)
    {
        if (audioSource == null)
            return;

        if (audioSource.isPlaying)
            await UniTaskUtils.WaitWhile(() => audioSource != null && audioSource.isPlaying);

        if (audioSourceQueue != null)
            audioSourceQueue.Enqueue(audioSource);
    }
}
