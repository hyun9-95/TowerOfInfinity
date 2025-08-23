using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class AbilitySlotUnit : BaseUnit<AbilitySlotUnitModel>
{
	#region Property
	#endregion

	#region Value
	[SerializeField]
	private AddressableLoader activeThumbnailLoader;

	[SerializeField]
	private AddressableLoader passiveThumbnailLoader;

	private List<AbilityThumbnailUnit> acitveSlotUnits;
	private List<AbilityThumbnailUnit> passiveSlotUnits;
    #endregion

    #region Function
    public override async UniTask ShowAsync()
    {
		if (acitveSlotUnits == null)
			acitveSlotUnits = new List<AbilityThumbnailUnit>(IntDefine.MAX_ABILITY_SLOT_COUNT);

		if (passiveSlotUnits == null)
			passiveSlotUnits = new List<AbilityThumbnailUnit>(IntDefine.MAX_ABILITY_SLOT_COUNT);

		if (Model.AbilityThumbUnitModelsBySlotType.TryGetValue(AbilitySlotType.Active, out var activeModels))
			await ShowAbilitySlotUnits(activeThumbnailLoader, acitveSlotUnits, activeModels);
		
		if (Model.AbilityThumbUnitModelsBySlotType.TryGetValue(AbilitySlotType.Passive, out var passiveModels))
			await ShowAbilitySlotUnits(passiveThumbnailLoader, passiveSlotUnits, passiveModels);
    }

	private async UniTask ShowAbilitySlotUnits(AddressableLoader loader, List<AbilityThumbnailUnit> slotUnits, IReadOnlyList<AbilityThumbnailUnitModel> modelList)
	{
        for (int i = 0; i < modelList.Count; i++)
        {
            if (i >= slotUnits.Count)
			{
				var newUnit = await loader.InstantiateAsyc<AbilityThumbnailUnit>();
				slotUnits.Add(newUnit);
            }

            slotUnits[i].SetModel(modelList[i]);
            await slotUnits[i].ShowAsync();
        }
    }
	#endregion
}
