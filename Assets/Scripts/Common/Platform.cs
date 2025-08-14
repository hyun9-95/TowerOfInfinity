using UnityEngine;

public class Platform
{
	#region Property
	public PlatformType Type { get; private set; }
    public bool IsMobile => Type is PlatformType.Android or PlatformType.IOS;
    #endregion

    #region Value
    #endregion

    #region Function
    public void SetPlatformType(PlatformType type)
    {
        Type = type;
    }
    #endregion
}
