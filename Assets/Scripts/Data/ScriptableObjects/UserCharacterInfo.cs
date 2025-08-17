using System;

public class UserCharacterInfo
{
    #region Property
    public MainCharacterInfo MainCharacterInfo { get; private set; }
    public SubCharacterInfo[] SubCharacterInfos { get; private set; }
    public SubCharacterInfo[] CurrentDeck { get; private set; } = new SubCharacterInfo[3];
    #endregion

    #region Function
    public void SetMainCharacterInfo(MainCharacterInfo mainCharacterInfo)
    {
        MainCharacterInfo = mainCharacterInfo;
    }

    public void SetSubCharacterInfos(SubCharacterInfo[] subCharacterInfos)
    {
        SubCharacterInfos = subCharacterInfos;
    }

    public void UpdateCurrentDeck()
    {
        if (SubCharacterInfos == null)
            return;

        Array.Clear(CurrentDeck, 0, CurrentDeck.Length);

        foreach (var subCharacter in SubCharacterInfos)
        {
            if (subCharacter == null)
                continue;

            int slotIndex = subCharacter.SlotIndex;
            
            if (slotIndex >= 0 && slotIndex < CurrentDeck.Length)
            {
                if (CurrentDeck[slotIndex] != null)
                {
                    Logger.Error($"중복 인덱스 ! => {subCharacter.CharacterDataId}");
                    continue;
                }

                CurrentDeck[slotIndex] = subCharacter;
            }
        }
    }

    #endregion
}