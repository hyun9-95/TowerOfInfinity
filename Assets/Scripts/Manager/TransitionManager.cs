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
        await transitions[(int)transitionType].In();
    }

    public async UniTask Out(TransitionType transitionType)
    {
        await transitions[(int)transitionType].Out();
    }
}