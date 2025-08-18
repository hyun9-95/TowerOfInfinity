using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimatorController
{
    public CharacterAnimState AnimState => animState;

    private Animator animator;

    private CharacterAnimState animState;

    public void SetAnimator(Animator anim)
    {
        animator = anim;
    }

    public void SetAnimState(CharacterAnimState state)
    {
        animState = state;
        animator.SetInteger(StringDefine.CHARACTER_ANIM_STATE_KEY, (int)state);
    }
}
