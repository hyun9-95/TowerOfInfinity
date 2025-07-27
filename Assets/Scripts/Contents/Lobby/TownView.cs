using UnityEngine;

public class TownView : BaseView
{
    public TownViewModel Model => GetModel<TownViewModel>();

    public void OnClickCustomization()
    {
        Model.OnClickCustomization();
    }
}
