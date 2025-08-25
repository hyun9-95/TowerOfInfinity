using System;
using TMPro;
using UnityEngine;

public class LocalizationTextSupport : MonoBehaviour, IObserver
{
    [SerializeField]
    private LocalizationDefine localizationDefine;

    [SerializeField]
    private TextMeshProUGUI textMeshPro;

    [SerializeField]
    private bool useObserver = false;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (textMeshPro == null)
        {
            textMeshPro = GetComponent<TextMeshProUGUI>();

            if (textMeshPro == null)
                textMeshPro = gameObject.AddComponent<TextMeshProUGUI>();
        }
    }
#endif

    private void OnEnable()
    {
        Translate();

        if (useObserver)
            ObserverManager.AddObserver(LocalizationObserverID.Changed, this);
    }

    private void OnDisable()
    {
        if (useObserver)
            ObserverManager.RemoveObserver(LocalizationObserverID.Changed, this);
    }

    private void Translate()
    {
        textMeshPro.SafeSetText(LocalizationManager.GetLocalization(localizationDefine));
    }

    void IObserver.HandleMessage(Enum observerMessage, IObserverParam observerParam)
    {
        if (observerMessage is not LocalizationObserverID.Changed)
            return;

        Translate();
    }
}
