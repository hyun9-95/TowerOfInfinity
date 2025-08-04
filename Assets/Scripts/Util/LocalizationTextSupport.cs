using TMPro;
using UnityEngine;

public class LocalizationTextSupport : MonoBehaviour
{
    [SerializeField]
    private LocalizationDefine localizationDefine;

    [SerializeField]
    private TextMeshProUGUI textMeshPro;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (textMeshPro == null)
            textMeshPro = GetComponent<TextMeshProUGUI>();
    }
#endif

    private void OnEnable()
    {
        textMeshPro.text = LocalizationManager.GetLocalization(localizationDefine);
    }
}
