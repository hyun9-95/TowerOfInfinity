using UnityEngine;

public class ResetAnimStateBehaviour : StateMachineBehaviour
{
    public enum ResetTiming
    {
        Enter,
        Finish,
    }

    public ResetTiming timing;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (timing == ResetTiming.Enter)
            animator.SetInteger(StringDefine.CHARACTER_ANIM_STATE_KEY, 0);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (timing == ResetTiming.Finish && stateInfo.normalizedTime >= 0.99f)
            animator.SetInteger(StringDefine.CHARACTER_ANIM_STATE_KEY, 0);
    }
}
