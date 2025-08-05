using System;
using UnityEngine;

public class BattleExpGainerModel : IBaseUnitModel
{
    public int Level => Owner.Level;

    public CharacterUnitModel Owner { get; private set; }

    public Action<float> OnExpGain { get; set; }

    public void SetOwner(CharacterUnitModel owner)
    {
        Owner = owner;
    }

    public void SetOnExpGain(Action<float> action)
    {
        OnExpGain = action;
    }
}
