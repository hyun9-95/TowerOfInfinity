using System;
using UnityEngine;

public class CollisionDetectorModel
{
    #region Property
    public Action<Collider2D> OnTriggerEnter { get; set; }
    public Action<Collider2D> OnTriggerExit { get; set; }
    #endregion

    #region Value
    #endregion

    #region Function
    public void SetOnTriggerEnter(Action<Collider2D> action)
    {
        OnTriggerEnter = action;
    }
    public void SetOnTriggerExit(Action<Collider2D> action)
    {
        OnTriggerExit = action;
    }
    #endregion



}
