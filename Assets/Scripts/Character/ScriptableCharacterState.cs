using UnityEngine;

public abstract class ScriptableCharacterState : ScriptableObject
{
    public abstract CharacterAnimState AnimState { get; }

    public abstract int Priority { get; }

    public abstract bool CheckEnterCondition(CharacterUnitModel model);

    public virtual bool CheckExitCondition(CharacterUnitModel model)
    {
        return false;
    }

    public virtual void OnStateAction(CharacterUnitModel model)
    {
    }

    public virtual void OnEnterState(CharacterUnitModel model)
    {
    }

    public virtual void OnExitState(CharacterUnitModel model)
    {
    }

    protected float GetAnimationDelay(CharacterAnimState state)
    {
        return state switch
        {
            CharacterAnimState.Attack => 0.12f,
            _ => 0f,
        };
    }
}
