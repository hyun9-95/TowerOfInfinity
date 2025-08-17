public abstract class SoloScriptableState : ScriptableCharacterState
{
    private CharacterUnitModel owner;

    public void SetOwner(CharacterUnitModel model)
    {
        owner = model;
        OnSetOwner(model);
    }

    protected virtual void OnSetOwner(CharacterUnitModel model)
    {
    }

    protected CharacterUnitModel GetOwner()
    {
        return owner;
    }

    public override bool CheckEnterCondition(CharacterUnitModel model)
    {
        if (!CheckOwner(model))
            return false;

        return CheckEnterConditionInternal(model);
    }

    public override bool CheckExitCondition(CharacterUnitModel model)
    {
        if (!CheckOwner(model))
            return false;

        return CheckExitConditionInternal(model);
    }

    public override void OnStateAction(CharacterUnitModel model)
    {
        if (!CheckOwner(model))
            return;

        OnStateActionInternal(model);
    }

    public override void OnEnterState(CharacterUnitModel model)
    {
        if (!CheckOwner(model))
            return;

        OnEnterStateInternal(model);
    }

    public override void OnExitState(CharacterUnitModel model)
    {
        if (!CheckOwner(model))
            return;

        OnExitStateInternal(model);
    }

    private bool CheckOwner(CharacterUnitModel model)
    {
        if (owner != null && owner != model)
        {
            if (model.Transform != null)
                Logger.Error($"SoloScriptableState를 중복 사용중 ! {model.Transform.gameObject.name}");
            return false;
        }

        return true;
    }

    protected abstract bool CheckEnterConditionInternal(CharacterUnitModel model);
    protected virtual bool CheckExitConditionInternal(CharacterUnitModel model) { return false; }
    protected virtual void OnStateActionInternal(CharacterUnitModel model) { }
    protected virtual void OnEnterStateInternal(CharacterUnitModel model) { }
    protected virtual void OnExitStateInternal(CharacterUnitModel model) { }

    private void OnDisable()
    {
        owner = null;
    }
}