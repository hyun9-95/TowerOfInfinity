using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class Transition : MonoBehaviour
{
    [SerializeField]
    private Animator inAnimator;

    [SerializeField]
    private CanvasGroup inCanvasGroup = null;

    [SerializeField]
    private Animator outAnimator;

    [SerializeField]
    private CanvasGroup outCanvasGroup = null;

    private void Awake()
    {
        inAnimator.speed = 0;
        outAnimator.speed = 0;
        inCanvasGroup.alpha = 0;
        outCanvasGroup.alpha = 0;

        inAnimator.gameObject.SafeSetActive(true);
        outAnimator.gameObject.SafeSetActive(true);
    }

    public async UniTask In()
    {
        inAnimator.Play(inAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash, 0, 0f);
        inAnimator.speed = 1;

        inCanvasGroup.alpha = 1;

        await UniTask.NextFrame();

        outCanvasGroup.alpha = 0;

        await UniTask.WaitUntil(() => inAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

        inAnimator.speed = 0;
    }

    public async UniTask Out()
    {
        outAnimator.Play(outAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash, 0, 0f);
        outAnimator.speed = 1;

        outCanvasGroup.alpha = 1;

        await UniTask.NextFrame();

        inCanvasGroup.alpha = 0;

        await UniTask.WaitUntil(() => outAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

        outAnimator.speed = 0;
    }
}