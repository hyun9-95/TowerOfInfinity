using UnityEngine;

public class AbilityThumbnailUnitModel : IBaseUnitModel
{
	#region Property
	public int Level { get; set; }
	public string IconPath { get; set; }
	#endregion
	
	#region Value
	#endregion
	
	#region Function
	public void SetLevel(int level)
	{
		Level = level;
	}

	public void SetIconPath(string path)
	{
		IconPath = path;
	}
	#endregion
}
