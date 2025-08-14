using UnityEditor;

[CustomEditor(typeof(ClickBlocker))]
public class ClickBlockerEditor : Editor
{
	#region Property
	#endregion
	
	#region Value
	#endregion
	
	#region Function
	public override void OnInspectorGUI()
	{
		EditorGUILayout.HelpBox("Click 막는 용도의 컴포넌트.", MessageType.Info);
    }
    #endregion
}
