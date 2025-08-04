using UnityEngine;

public class MainCharacterInfo : CharacterInfo
{
    #region Property
    public string MainCharacterPath => PathDefine.CHARACTER_MAIN_CHARACTER_PATH;
    public MainCharacterPartsInfo PartsInfo { get; private set; } = new MainCharacterPartsInfo();
    public CharacterRace CharacterRace { get; private set; }
    public int HairPartsId { get; private set; }
    #endregion
    #region Value
    public void SetCharacterRace(CharacterRace characterRace)
    {
        CharacterRace = characterRace;
    }

    public void SetHairPartsId(int hairDataId)
    {
        HairPartsId = hairDataId;
    }

    public void SetPartsInfo(MainCharacterPartsInfo partsInfo)
    {
        PartsInfo = partsInfo;
    }
    #endregion
}
