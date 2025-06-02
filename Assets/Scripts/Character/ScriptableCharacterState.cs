using UnityEngine;

public abstract class ScriptableCharacterState : ScriptableObject
{
    public abstract CharacterAnimState AnimState { get; }

    [Range(0, 100)]
    public abstract int Priority { get; }

    public abstract bool CheckEnterCondition(CharacterUnitModel model);

    public virtual bool CheckExitCondition(CharacterUnitModel model)
    {
        return false;
    }

    public abstract void OnStateAction(CharacterUnitModel model);

    public virtual void OnEnterState(CharacterUnitModel model)
    {
    }

    public virtual void OnExitState(CharacterUnitModel model)
    {
    }
}
