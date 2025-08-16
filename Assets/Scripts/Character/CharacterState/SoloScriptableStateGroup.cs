using UnityEngine;

[CreateAssetMenu(fileName = "SoloStateGroup", menuName = "Scriptable Objects/SoloStateGroup")]
public class SoloScriptableStateGroup : ScriptableCharacterStateGroup
{
	#region Property
	#endregion

	#region Value
	private CharacterUnit owner;
	#endregion
	
	#region Function
	public void SetOwner(CharacterUnit ownerValue)
	{
		if (ownerValue == null || ownerValue.Model == null)
			return;

		if (owner != null && owner != ownerValue)
		{
			Logger.Error($"SoloScriptableStateGroup 중복 Owner Set 시도");
			return;
		}

		owner = ownerValue;

		var model = owner.Model;

		for (int i = 0; i < stateList.Count; i++)
		{
			if (stateList[i] is SoloScriptableState soloState)
                soloState.SetOwner(model);
        }
	}
	#endregion
}
