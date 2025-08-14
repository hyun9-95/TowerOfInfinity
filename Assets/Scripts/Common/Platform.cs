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
    public void SetPlatform(PlatformType type)
    {
        Type = type;

        InitializePlatform();
    }

    private void InitializePlatform()
    {
        switch (Type)
        {
            case PlatformType.None:
                break;

            case PlatformType.StandAlone:
                break;

            case PlatformType.Android:
                break;

            case PlatformType.IOS:
                break;
        }
    }
    #endregion
}
