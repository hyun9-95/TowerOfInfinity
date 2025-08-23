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

		await ShowAbilitySlotUnits(activeThumbnailLoader, acitveSlotUnits, Model.ActiveAbilityThumbUnitModels);
		await ShowAbilitySlotUnits(passiveThumbnailLoader, passiveSlotUnits, Model.PassiveAbilityThumbUnitModels);
    }

	private async UniTask ShowAbilitySlotUnits(AddressableLoader loader, List<AbilityThumbnailUnit> acitveSlotUnits, IReadOnlyList<AbilityThumbnailUnitModel> modelList)
	{
        for (int i = 0; i < modelList.Count; i++)
        {
            if (acitveSlotUnits[i] == null)
                acitveSlotUnits[i] = await loader.LoadAsync<AbilityThumbnailUnit>();

            acitveSlotUnits[i].SetModel(modelList[i]);
            await acitveSlotUnits[i].ShowAsync();
        }
    }
	#endregion
}
