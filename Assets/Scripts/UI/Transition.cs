using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class Transition : MonoBehaviour
{
    public bool IsPlaying => isPlaying;

    [SerializeField]
    private Animator inAnimator;

    [SerializeField]
    private CanvasGroup inCanvasGroup = null;

    [SerializeField]
    private Animator outAnimator;

    [SerializeField]
    private CanvasGroup outCanvasGroup = null;

    [SerializeField]
    private float speed = 1;

    private bool isPlaying = false;

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
        isPlaying = true;
        inCanvasGroup.alpha = 1;
        inAnimator.Play(inAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash, 0, 0f);
        inAnimator.speed = speed;

        await UniTask.NextFrame();

        outCanvasGroup.alpha = 0;

        await UniTask.WaitUntil(() => inAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

        inAnimator.speed = 0;
        isPlaying = false;
    }

    public async UniTask Out()
    {
        isPlaying = true;
        outCanvasGroup.alpha = 1;
        outAnimator.Play(outAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash, 0, 0f);
        outAnimator.speed = speed;

        await UniTask.NextFrame();

        inCanvasGroup.alpha = 0;

        await UniTask.WaitUntil(() => outAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

        outAnimator.speed = 0;
        isPlaying = false;
    }
}