using System;

public class ColorSelectUnitModel : IBaseUnitModel
{
    #region Property
    public string PreviewImagePath { get; private set; }

    /// <summary>
    /// return : Color hex code
    /// </summary>
    public Action<string> OnColorConfirmed { get; private set; }
    #endregion

    #region Value
    #endregion


    #region Function
    public void SetPreviewImage(string path)
    {
        PreviewImagePath = path;
    }

    public void SetOnColorConfirmed(Action<string> onColorConfirmed)
    {
        OnColorConfirmed = onColorConfirmed;
    }
    #endregion
}
