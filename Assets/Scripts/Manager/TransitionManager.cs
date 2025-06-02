#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TransitionManager : BaseMonoManager<TransitionManager>
{
    [SerializeField]
    private Transition[] transitions;

    public async UniTask In(TransitionType transitionType)
    {
        await transitions[(int)transitionType].In();
    }

    public async UniTask Out(TransitionType transitionType)
    {
        await transitions[(int)transitionType].Out();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
            In(TransitionType.Default).Forget();

        if (Input.GetKeyDown(KeyCode.F2))
            Out(TransitionType.Default).Forget();
    }
}