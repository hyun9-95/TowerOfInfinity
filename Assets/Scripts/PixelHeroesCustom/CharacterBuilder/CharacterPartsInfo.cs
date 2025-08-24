using Newtonsoft.Json;
using UnityEngine;

[System.Serializable]
public class CharacterPartsInfo
{
    [JsonProperty]
    public int PartsDataId { get; set; }

    [JsonProperty]
    public string ColorCode { get; set; }

    [JsonProperty]
    public Vector3 HSV { get; set; }

    public CharacterPartsInfo()
    {
        PartsDataId = 0;
        ColorCode = string.Empty;
        HSV = Vector3.zero;
    }

    public CharacterPartsInfo(int partsDataId, string colorCode = "", Vector3 hsv = default)
    {
        PartsDataId = partsDataId;
        ColorCode = string.IsNullOrEmpty(colorCode) ? string.Empty : colorCode;
        HSV = hsv;
    }

    public CharacterPartsInfo(DataCharacterParts partsData, string colorCode = "", Vector3 hsv = default)
    {
        if (partsData.IsNullOrEmpty())
        {
            PartsDataId = 0;
            ColorCode = string.Empty;
            HSV = Vector3.zero;
            return;
        }

        PartsDataId = partsData.Id;
        ColorCode = string.IsNullOrEmpty(colorCode) ? string.Empty : colorCode;
        HSV = hsv;
    }

    public DataCharacterParts GetPartsData()
    {
        if (PartsDataId == 0)
            return default;
            
        return DataManager.Instance.GetDataById<DataCharacterParts>(PartsDataId);
    }

    public bool IsValid()
    {
        if (PartsDataId <= 0)
            return false;

        var partsData = GetPartsData();
        return !partsData.IsNullOrEmpty();
    }

    public bool HasColorCustomization()
    {
        return !string.IsNullOrEmpty(ColorCode) || HSV != Vector3.zero;
    }

    public string GetFormattedPartsName()
    {
        var partsData = GetPartsData();
        if (partsData.IsNullOrEmpty())
            return string.Empty;

        var partsName = partsData.PartsName;
        
        if (string.IsNullOrEmpty(partsName))
            return string.Empty;

        var hasColorCode = !string.IsNullOrEmpty(ColorCode);
        var hasHSV = HSV != Vector3.zero;

        if (hasColorCode && hasHSV)
            return $"{partsName}{ColorCode}/{HSV.x}:{HSV.y}:{HSV.z}";
        
        if (hasColorCode)
            return $"{partsName}{ColorCode}";
        
        if (hasHSV)
            return $"{partsName}/{HSV.x}:{HSV.y}:{HSV.z}";
        
        return partsName;
    }
}