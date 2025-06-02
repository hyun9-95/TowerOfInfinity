using System;
using UnityEngine;

public class IntroViewModel : IBaseViewModel
{
    public enum LoadingState
    {
        ResourceLoading,
        DataLoading,
        UserLoading,
    }

    public BaseDataLoader DataLoader => LocalDataLoader;

    public LocalDataLoader LocalDataLoader { get; private set; }

    public LoadDataType LoadDataType { get; private set; }

    public Action OnCompleteLoading { get; private set; }

    public LoadingState CurrentLoadingState { get; private set; }

    public void SetLoadingState(LoadingState loadingState)
    {
        CurrentLoadingState = loadingState;
    }

    public void SetLoadDataType(LoadDataType loadDataType)
    {
        LoadDataType = loadDataType;
    }

    public void SetLocalDataLoader(LocalDataLoader localDataLoader)
    {
        LocalDataLoader = localDataLoader;
    }

    public void SetOnComplteLoading(Action onCompleteLoading)
    {
        OnCompleteLoading = onCompleteLoading;
    }

    public string GetLoadingProgressText()
    {
        switch (CurrentLoadingState)
        {
            case LoadingState.ResourceLoading:
                return "Loading Resources...";

            case LoadingState.DataLoading:
                return DataLoader.CurrentProgressString;

            case LoadingState.UserLoading:
                return "Loading Users...";
        }

        return "Loading...";
    }
}
