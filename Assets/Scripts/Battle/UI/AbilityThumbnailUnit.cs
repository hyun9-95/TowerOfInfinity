using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class AbilityThumbnailUnit : BaseUnit<AbilityThumbnailUnitModel>
{
	#region Property
	#endregion

	#region Value
	[SerializeField]
	private Image icon;

	[SerializeField]
	private Slider levelSlider;
    #endregion

    #region Function
    public override async UniTask ShowAsync()
    {
		await icon.SafeLoadAsync(Model.IconPath);
		levelSlider.value = Model.Level;
    }
	#endregion
}
