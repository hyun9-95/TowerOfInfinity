using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	#region Property
	public bool IsClicked => isClicked;
	public bool IsCoolTime => isCoolTime;
    public ActionInput InputType => inputType;
    #endregion

    #region Value
    [SerializeField]
	private ActionInput inputType;

	[SerializeField]
	private Image buttonImage;

	[SerializeField]
	private Image coolDownDim;

	[SerializeField]
	private Sprite defaultSprite;

	[SerializeField]
	private Sprite clickedSprite;

	[SerializeField]
	private ButtonSoundType buttonSoundType;

    private float coolTime = 0f;
	private bool isClicked = false;
	private bool isCoolTime = false;
    #endregion

    #region Function
	public void Enable(float coolTime = 0)
	{
		this.coolTime = coolTime;
		coolDownDim.gameObject.SafeSetActive(false);
		buttonImage.sprite = defaultSprite;
		isClicked = false;
		isCoolTime = false;

		gameObject.SafeSetActive(true);
    }

	private async UniTask CoolDownAsync(float coolTime)
	{
		var elapsedTime = 0f;

		coolDownDim.gameObject.SafeSetActive(true);
		coolDownDim.fillAmount = 1f;
		isCoolTime = true;

        while (elapsedTime < coolTime)
		{
			elapsedTime += Time.deltaTime;

			var fillAmount = 1f - (elapsedTime / coolTime);
			coolDownDim.fillAmount = fillAmount;
			await UniTask.NextFrame(TokenPool.Get(GetHashCode()));	
        }

		isCoolTime = false;
        coolDownDim.fillAmount = 0f;
        coolDownDim.gameObject.SafeSetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
		isClicked = true;
		OnAction();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isClicked = false;
		buttonImage.sprite = defaultSprite;
    }

	public void OnAction(bool restoreSprite = false)
	{
        if (isCoolTime)
            return;

        buttonImage.sprite = clickedSprite;
        SoundManager.Instance.PlayButtonSound(buttonSoundType);

        if (coolTime > 0)
            CoolDownAsync(coolTime).Forget();

		Logger.Log($"Action Input => {inputType}");

		if (restoreSprite)
		{
            UniTaskUtils.DelayAction(0.1f, () =>
            {
				buttonImage.sprite = defaultSprite;
            }, TokenPool.Get(GetHashCode())).Forget();
        }	
    }
    #endregion
}
