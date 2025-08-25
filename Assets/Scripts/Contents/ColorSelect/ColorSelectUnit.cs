using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ColorSelectUnit : BaseUnit<ColorSelectUnitModel>
{
    [SerializeField]
    private Image colorPreview;

    [SerializeField]
    private Slider colorSlider_R;

    [SerializeField]
    private Slider colorSlider_G;

    [SerializeField]
    private Slider colorSlider_B;

    [SerializeField]
    private Button confirmButton;

    private void Awake()
    {
        SetUpEventListener();
    }

    public async override UniTask ShowAsync()
    {
        await colorPreview.SafeLoadAsync(Model.PreviewImagePath);
    }

    private void SetUpEventListener()
    {
        if (colorSlider_R)
            colorSlider_R.onValueChanged.AddListener(OnHairColorSliderChanged);

        if (colorSlider_G)
            colorSlider_G.onValueChanged.AddListener(OnHairColorSliderChanged);

        if (colorSlider_B)
            colorSlider_B.onValueChanged.AddListener(OnHairColorSliderChanged);

        if (confirmButton)
            confirmButton.onClick.AddListener(OnConfirmColor);
    }

    private void OnHairColorSliderChanged(float _)
    {
        UpdatePreviewColorFromSliders();
    }

    private void UpdatePreviewColorFromSliders()
    {
        if (colorPreview == null)
            return;

        float r = colorSlider_R != null ? colorSlider_R.value / 255f : colorPreview.color.r;
        float g = colorSlider_G != null ? colorSlider_G.value / 255f : colorPreview.color.g;
        float b = colorSlider_B != null ? colorSlider_B.value / 255f : colorPreview.color.b;

        var color = new Color(r, g, b, 1f);
        colorPreview.color = color;
    }

    public void OnConfirmColor()
    {
        string hex = $"#{ColorUtility.ToHtmlStringRGB(colorPreview.color)}";
        Model.OnColorConfirmed(hex);
    }
}
