using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

public abstract class BaseDataLoader : IBaseViewModel
{
    public enum State
    {
        LoadVersion,
        LoadJsonList,
        LoadJson,
        Success,
        Fail,
    }

    public T GetLoader<T>() where T : BaseDataLoader
    {
        return this is T ? (T)this : null;
    }

    public string CurrentProgressString
    {
        get
        {
            switch (CurrentState)
            {
                case State.LoadVersion:
                    return "Loading Version...";
                case State.LoadJsonList:
                    return "Loading JsonList...";
                case State.LoadJson:
                    return "Loading Json...";
                case State.Success:
                    return "Loading Success!";
                case State.Fail:
                    return "Loading Fail!";
                default:
                    return null;
            }
        }
    }

    public float CurrentProgressValue { get; protected set; } = 0f;

    public State CurrentState { get; protected set; }

    public bool IsLoading => CurrentState != State.Success && CurrentState != State.Fail;

    public Dictionary<string, string> DicJsonByFileName { get; protected set; } = new();

    public Action OnSuccessLoadData { get; private set; }

    public abstract UniTask LoadData();

    public void SetOnSuccessLoadData(Action action)
    {
        OnSuccessLoadData = action;
    }

    protected void ChangeState(State state)
    {
        CurrentState = state;
    }
}
