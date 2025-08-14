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
#if UNITY_EDITOR
    protected override void Reset()
    {
        base.Reset();
        UnityEditor.Events.UnityEventTools.AddPersistentListener(onClick, OnPlayButtonSound);
    }
#endif

    public void OnPlayButtonSound()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayButtonSound(playSoundType);
    }
    #endregion
}
