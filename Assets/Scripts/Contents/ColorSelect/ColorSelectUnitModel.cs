using System;

public class ColorSelectUnitModel : IBaseUnitModel
{
    #region Property
    public string PartsImagePath { get; private set; }

    public string PreviewImagePath { get; private set; }

    public string CurrentColor { get; private set; }

    /// <summary>
    /// return : Color hex code
    /// </summary>
    public Action<string> OnColorConfirmed { get; private set; }
    #endregion

    #region Value
    #endregion


    #region Function
    public void SetPartsImagePath(string path)
    {
        PartsImagePath = path;
    }

    public void SetPreviewImagePath(string path)
    {
        PreviewImagePath = path;
    }

    public void SetCurrntColor(string colorCode)
    {
        CurrentColor = colorCode;
    }

    public void SetOnColorConfirmed(Action<string> onColorConfirmed)
    {
        OnColorConfirmed = onColorConfirmed;
    }
    #endregion
}
