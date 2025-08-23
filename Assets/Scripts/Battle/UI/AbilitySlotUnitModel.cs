using System.Collections.Generic;

public class AbilitySlotUnitModel : IBaseUnitModel
{
	#region Property
	public IReadOnlyList<AbilityThumbnailUnitModel> ActiveAbilityThumbUnitModels => activeAbilityThumbs;
	public IReadOnlyList<AbilityThumbnailUnitModel> PassiveAbilityThumbUnitModels => passiveAbilityThubms;
	#endregion

	#region Value
	private List<AbilityThumbnailUnitModel> activeAbilityThumbs = new();
	private List<AbilityThumbnailUnitModel> passiveAbilityThubms = new();
	#endregion
	
	#region Function
	public void AddAbilityThumbUnitModel(AbilitySlotType slotType, AbilityThumbnailUnitModel model)
	{
		if (slotType == AbilitySlotType.Active)
			activeAbilityThumbs.Add(model);
		else
			passiveAbilityThubms.Add(model);
	}
	#endregion
}
