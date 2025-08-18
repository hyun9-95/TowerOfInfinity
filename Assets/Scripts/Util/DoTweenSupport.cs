using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class DoTweenSupport : MonoBehaviour
{
	#region Property
	#endregion

	#region Value
	[SerializeField]
	private DOTweenAnimation tweenAnimation;
    #endregion

    #region Function

#if UNITY_EDITOR
    private void Reset()
    {
        if (tweenAnimation == null)
			tweenAnimation = GetComponent<DOTweenAnimation>();
    }
#endif

    private void Awake()
    {
		if (tweenAnimation != null)
			tweenAnimation.DORewind();
    }

    public async UniTask ShowAsync()
	{
		tweenAnimation.DORestart();

		await tweenAnimation.tween.AsyncWaitForCompletion();
    }

	public void Show()
	{
        tweenAnimation.DORestart();
    }

	public async UniTask WaitCompletion()
	{
        await tweenAnimation.tween.AsyncWaitForCompletion();
    }

    public async UniTask HideAsync()
	{
		tweenAnimation.DOPlayBackwards();

		await tweenAnimation.tween.AsyncWaitForRewind();
	}
	#endregion
}
