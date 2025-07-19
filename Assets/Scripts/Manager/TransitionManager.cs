#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TransitionManager : BaseMonoManager<TransitionManager>
{
    [SerializeField]
    private Transition[] transitions;

    /// <summary>
    /// In 은 Flow에서만 사용하자.
    /// </summary>
    /// <param name="transitionType"></param>
    /// <returns></returns>
    public async UniTask In(TransitionType transitionType)
    {
        var transition = transitions[(int)transitionType];

        if (transition.IsPlaying)
            await UniTask.WaitUntil(() => !transition.IsPlaying);

        await transition.In();
        Logger.Success($"[Transition] In => {transitionType}");
    }

    public async UniTask Out(TransitionType transitionType)
    {
        var transition = transitions[(int)transitionType];

        if (transition.IsPlaying)
            await UniTask.WaitUntil(() => !transition.IsPlaying);

        await transition.Out();
        Logger.Success($"[Transition] Out => {transitionType}");
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.F1))
    //        In(TransitionType.Default).Forget();
    //
    //    if (Input.GetKeyDown(KeyCode.F2))
    //        Out(TransitionType.Default).Forget();
    //}
}