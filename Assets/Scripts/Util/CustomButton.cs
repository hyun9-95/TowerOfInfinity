using UnityEditor.Events;
using UnityEngine;
using UnityEngine.UI;

public class CustomButton : Button
{
	#region Property
	#endregion

	#region Value
	[SerializeField]
	private ButtonSoundType playSoundType;
    #endregion

    #region Function
    protected override void Reset()
    {
        base.Reset();

#if UNITY_EDITOR
        UnityEventTools.AddPersistentListener(onClick, OnPlayButtonSound);
#endif
    }

    public void OnPlayButtonSound()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayButtonSound(playSoundType);
    }
    #endregion
}
