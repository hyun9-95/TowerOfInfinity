using UnityEngine;

public class MainCharacterInfo : CharacterInfo
{
    #region Property
    public string MainCharacterPath => PathDefine.CHARACTER_MAIN_CHARACTER_PATH;
    public MainCharacterPartsInfo PartsInfo { get; private set; } = new MainCharacterPartsInfo();
    #endregion
    #region Value
    public void SetPartsInfo(MainCharacterPartsInfo partsInfo)
    {
        PartsInfo = partsInfo;
    }
    #endregion
}
