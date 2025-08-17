using System.Text;

public static class GlobalStringBuilder
{
	#region Property
	#endregion

	#region Value
	private static StringBuilder sb = new();
	#endregion
	
	#region Function
	public static StringBuilder Get()
	{
		sb.Clear();

		return sb;
	}
	#endregion
}
