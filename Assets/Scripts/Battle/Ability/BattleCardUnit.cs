#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleCardUnit : BaseUnit<BattleCardUnitModel>
{
	#region Property
	#endregion

	#region Value
	[SerializeField]
	private Image tierFrame;

	[SerializeField]
	private Image icon;

	[SerializeField]
	private TextMeshProUGUI nameText;

	[SerializeField]
	private TextMeshProUGUI descriptionText;

	[SerializeField]
	private CustomButton customButton;

	[SerializeField]
	private DOTweenAnimation cardAppearTween;

	[SerializeField]
	private DOTweenAnimation cardSelectTween;
    #endregion

    #region Function

    private void Awake()
    {
		cardAppearTween.DORewind();
    }

    private void OnDisable()
    {
        cardAppearTween.DORewind();
		cardSelectTween.DORewind();
    }

    public async UniTask LoadAsync()
	{
        var tasks = new UniTask[]
        {
            ShowTierFrame(),
            ShowIcon(),
        };

        ShowTexts();

        await UniTask.WhenAll(tasks);
    }

    public override async UniTask ShowAsync()
    {
		cardAppearTween.DORestart();
		await cardAppearTween.tween.AsyncWaitForCompletion();
    }

	public async UniTask HideAsync()
	{
		cardAppearTween.DOPlayBackwards();
		await cardAppearTween.tween.AsyncWaitForRewind();
	}

	public async UniTask ShowSelectTweenAsync()
	{
		cardSelectTween.DORestart();
		await cardSelectTween.tween.AsyncWaitForCompletion();
	}

	private async UniTask ShowTierFrame()
	{
		var path = Model.GetTierFramePath();

		if (string.IsNullOrEmpty(path))
			return;

		await tierFrame.SafeLoadAsync(path);
	}

	private async UniTask ShowIcon()
	{
		if (string.IsNullOrEmpty(Model.IconPath))
		{
			icon.gameObject.SafeSetActive(false);
			return;
		}

		await icon.SafeLoadAsync(Model.IconPath);
		icon.gameObject.SafeSetActive(true);
	}

	private void ShowTexts()
	{
		nameText.SafeSetText(Model.NameText);
		descriptionText.SafeSetText(Model.DescriptionText);
	}
    #endregion
}
