using System;

public class TownViewModel : IBaseViewModel
{
    public Action OnClickCustomization { get; private set; }

    public void SetOnClickCustomization(Action onClickCustomization)
    {
        OnClickCustomization = onClickCustomization;
    }
}
