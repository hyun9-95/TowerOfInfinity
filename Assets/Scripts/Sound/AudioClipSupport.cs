using Cysharp.Threading.Tasks;
using UnityEngine;

public class AudioClipSupport : MonoBehaviour
{
	public enum AutoPlayTiming
	{
		None,
		OnEnable,
		OnDisable,
	}

	#region Property
	#endregion

	#region Value
	[SerializeField]
	private AutoPlayTiming autoPlayTiming;

	[SerializeField]
	private SoundType soundType;

	[SerializeField]
	private string autoPlayPath;

	[SerializeField]
	private string[] audioClipPathArray;

	[Header("Enable 이후 중복 재생 방지. Disable시 초기화")]
	[SerializeField]
	private bool disallowMultiplePlay;

	private bool[] playFlag;
    #endregion

    private void Awake()
    {
		if (disallowMultiplePlay)
			playFlag = new bool[audioClipPathArray.Length];
    }

    private void OnEnable()
    {
        if (autoPlayTiming == AutoPlayTiming.OnEnable)
            AutoPlay();
    }

    private void OnDisable()
    {
        if (autoPlayTiming == AutoPlayTiming.OnDisable)
            AutoPlay();

        if (disallowMultiplePlay)
        {
            for (int i = 0; i < playFlag.Length; i++)
                playFlag[i] = false;
        }
    }

    #region Function
    public void PlayPathFromArray(int index)
	{
		if (audioClipPathArray == null || index >= audioClipPathArray.Length)
			return;

		string path = audioClipPathArray[index];

		if (string.IsNullOrEmpty(path))
			return;

		if (disallowMultiplePlay)
		{
			if (playFlag[index])
			{
				// 중복 재생 비허용
				return;
			}
			else
			{
				playFlag[index] = true;
			}
		}

		if (soundType == SoundType.Bgm)
		{
			Logger.Log("BGM은 AudioClipSupport로 재생 불가");
			return;
		}	

		SoundManager.Instance.PlaySound(soundType, path).Forget();
	}

    private void AutoPlay()
	{
		if (string.IsNullOrEmpty(autoPlayPath))
			return;

        if (soundType == SoundType.Bgm)
        {
            Logger.Log("BGM은 AudioClipSupport로 재생 불가");
            return;
        }

        SoundManager.Instance.PlaySound(soundType, autoPlayPath).Forget();
	}
	#endregion
}
