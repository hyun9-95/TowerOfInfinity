using AYellowpaper.SerializedCollections;
using UnityEngine;

public class ResetAnimStateBehaviour : StateMachineBehaviour
{
    public enum ResetTiming
    {
        Enter,
        Finish,
    }

    public ResetTiming timing;

    public SerializedDictionary<string, float> normalizeTimeByTag;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (timing == ResetTiming.Enter)
            animator.SetInteger(StringDefine.CHARACTER_ANIM_STATE_KEY, 0);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.normalizedTime <= 0.749)
            return;

        float checkNormalizeTime = 0.99f;

        if (normalizeTimeByTag != null)
        {
            if (normalizeTimeByTag.TryGetValue(animator.tag, out var normalizeTime))
                checkNormalizeTime = normalizeTime;
        }

        if (timing == ResetTiming.Finish && stateInfo.normalizedTime >= checkNormalizeTime)
            animator.SetInteger(StringDefine.CHARACTER_ANIM_STATE_KEY, 0);
    }
}
