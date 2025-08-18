using UnityEngine;
using UnityEngine.UI;

public class LoadingBar : BaseUnit
{
	#region Property
	#endregion

	#region Value
	[SerializeField]
	private Slider loadingBar;

    [SerializeField]
	private TMPro.TextMeshProUGUI loadingText;

	[SerializeField]
	private GameObject completeObject;
    #endregion

    #region Function
	public void SetLoadingProgressText(string text)
	{
		loadingText.SafeSetText(text);
    }

	public void SetLoadingProgress(float progress)
	{
		loadingBar.value = progress;
    }

	public void SetComplete(bool value)
	{
		if (value)
		{
			loadingBar.gameObject.SafeSetActive(false);
			completeObject.SafeSetActive(true);
        }
		else
		{
			loadingBar.gameObject.SafeSetActive(true);
			completeObject.SafeSetActive(false);
        }
	}
    #endregion
}
