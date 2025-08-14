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
    #endregion
}
