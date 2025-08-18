using System;

public interface IObserver
{
    public abstract void HandleMessage(Enum observerMessage, IObserverParam observerParam);
}