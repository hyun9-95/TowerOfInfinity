using System;
using System.Collections.Generic;
using UnityEngine.U2D.Animation;

public class CharacterCustomizationViewModel : IBaseViewModel
{
    public List<string> Parts { get; private set; }
    public Action<int, string> OnPartChanged { get; set; }
    public SpriteLibrary SpriteLibrary { get; private set; }

    public void InitializeParts(int count)
    {
        Parts = new List<string>(new string[count]);
        for (int i = 0; i < count; i++)
        {
            Parts[i] = string.Empty;
        }
    }

    public void SetPlayerSpriteLibrary(SpriteLibrary spriteLibrary)
    {
        SpriteLibrary = spriteLibrary;
    }

    public void SetSpriteLibrary(SpriteLibrary spriteLibrary)
    {
        SpriteLibrary = spriteLibrary;
    }

    public void SetUserCharacterInfo(UserCharacterAppearanceInfo characterInfo)
    {
        if (characterInfo != null && characterInfo.parts != null)
        {
            InitializeParts(characterInfo.parts.Length);
            for (int i = 0; i < characterInfo.parts.Length; i++)
            {
                Parts[i] = characterInfo.parts[i] ?? string.Empty;
            }
        }
    }

    public void ChangePart(int index, string value)
    {
        if (index >= 0 && index < Parts.Count)
        {
            var oldValue = Parts[index];
            Parts[index] = value ?? string.Empty;
            
            // 값이 실제로 변경된 경우에만 이벤트 발생
            if (oldValue != Parts[index])
            {
                OnPartChanged?.Invoke(index, Parts[index]);
            }
        }
    }

    public string GetPart(int index)
    {
        if (index >= 0 && index < Parts.Count)
        {
            return Parts[index];
        }
        return string.Empty;
    }

    public string GetPart(CharacterPartsName partName)
    {
        int index = (int)partName;
        return GetPart(index);
    }

    public void SetPart(CharacterPartsName partName, string value)
    {
        int index = (int)partName;
        ChangePart(index, value);
    }
}
