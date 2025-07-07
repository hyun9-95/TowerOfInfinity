using System.Collections.Generic;
using UnityEngine;

public class BattleTeam
{
    public int CurrentIndex { get; private set; }
    public List<CharacterUnit> CharacterUnits { get; private set; }

    public CharacterUnit CurrentCharacter
    {
        get
        {
            if (CurrentIndex < 0 || CurrentIndex >= CharacterUnits.Count)
                return null;

            return CharacterUnits[CurrentIndex];
        }
    }

    public void SetCharacterUnits(List<CharacterUnit> characterUnits)
    {
        CharacterUnits = characterUnits;
    }

    public void SetCurrentIndex(int index)
    {
        if (CharacterUnits == null)
            return;

        if (index < 0 || index >= CharacterUnits.Count)
        {
            Logger.Error("Invalid index.");
            return;
        }

        CurrentIndex = index;
    }

    public CharacterUnit GetCharacterUnit(int index)
    {
        if (CharacterUnits == null || CharacterUnits.Count >= index)
            return null;

        return CharacterUnits[index];
    }
}
