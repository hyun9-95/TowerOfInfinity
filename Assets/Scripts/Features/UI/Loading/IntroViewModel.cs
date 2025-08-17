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

    public BaseDataLoader DataLoader
    {
        get
        {
            if (EditorDataLoader != null)
                return EditorDataLoader;
            
            if (AddressableDataLoader != null)
                return AddressableDataLoader;
            
            return null;
        }
    }

    public EditorDataLoader EditorDataLoader { get; private set; }
    
    public AddressableDataLoader AddressableDataLoader { get; private set; }

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

    public void SetEditorDataLoader(EditorDataLoader editorDataLoader)
    {
        EditorDataLoader = editorDataLoader;
    }

    public void SetAddressableDataLoader(AddressableDataLoader addressableDataLoader)
    {
        AddressableDataLoader = addressableDataLoader;
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
