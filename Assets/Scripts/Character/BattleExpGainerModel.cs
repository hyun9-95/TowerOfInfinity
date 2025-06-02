using System;
using UnityEngine;

public class BattleExpGainerModel : IBaseUnitModel
{
    public int Level { get; private set; }

    public CharacterUnitModel Owner { get; private set; }

    public Action<float> OnExpGain { get; set; }

    public void SetLevel(int level)
    {
        Level = level;
    }

    public void AddLevel(int level)
    {
        Level += level;
    }

    public void SetOwner(CharacterUnitModel owner)
    {
        Owner = owner;
    }

    public void SetOnExpGain(Action<float> action)
    {
        OnExpGain = action;
    }
}
