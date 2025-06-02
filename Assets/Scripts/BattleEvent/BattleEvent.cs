public abstract class BattleEvent
{
    public bool IsValid => Model != null && !Model.Sender.IsDead && !Model.Receiver.IsDead;
    protected BattleEventModel Model { get; private set; }

    public void SetModel(BattleEventModel model)
    {
        Model = model;
    }

    public void Reset()
    {
        Model = null;
    }

    public abstract void Process();

    public virtual void ReturnToPool()
    {
        BattleEventFactory.ReturnToPool(Model);
        BattleEventFactory.ReturnToPool(this);
    }
}
