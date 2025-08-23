using System.Collections.Generic;

public class AbilitySlotUnitModel : IBaseUnitModel
{
	#region Property
	public IReadOnlyDictionary<AbilitySlotType, List<AbilityThumbnailUnitModel>> AbilityThumbUnitModelsBySlotType => abilityThumbsBySlotType;
	#endregion

	#region Value
	private Dictionary<AbilitySlotType, List<AbilityThumbnailUnitModel>> abilityThumbsBySlotType = new();
	#endregion
	
	#region Function
	public void SyncWithAbilityProcessor(IReadOnlyDictionary<AbilitySlotType, List<Ability>> abilitySlotDic)
	{
		abilityThumbsBySlotType.Clear();

		foreach (var kvp in abilitySlotDic)
		{
			var slotType = kvp.Key;
			var abilities = kvp.Value;

			if (abilities == null || abilities.Count == 0)
				continue;

			var thumbModelList = new List<AbilityThumbnailUnitModel>();

			foreach (var ability in abilities)
			{
				if (ability?.Model?.AbilityData == null)
					continue;

				var thumbModel = new AbilityThumbnailUnitModel();
				thumbModel.SetLevel(ability.Model.Level);
				thumbModel.SetIconPath(ability.Model.AbilityData.IconPath);

				thumbModelList.Add(thumbModel);
			}

			abilityThumbsBySlotType[slotType] = thumbModelList;
		}
	}
	#endregion
}
