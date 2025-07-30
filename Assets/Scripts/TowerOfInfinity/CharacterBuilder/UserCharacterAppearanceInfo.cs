using System;

[Serializable]
public class UserCharacterAppearanceInfo
{
    public string[] parts = new string[Enum.GetNames(typeof(CharacterPartsType)).Length];
}
