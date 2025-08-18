using UnityEngine;
using TMPro;

public class SimpleTextUnit : BaseUnit<SimpleTextUnitModel>
{
    [SerializeField]
    private RectTransform rect;

    [SerializeField]
    private TextMeshProUGUI text;

    public override void Show()
    {
        SetAnchoredPosition();
        text.SafeSetText(Model.Text);

        base.Show();
    }

    private void SetAnchoredPosition()
    {
        if (rect == null)
            return;

        switch (Model.TextAnchor)
        {
            case SimpleTextUnitModel.Anchor.TopCenter:
                rect.anchorMin = new Vector2(0.5f, 0.95f);
                rect.anchorMax = new Vector2(0.5f, 0.95f);
                rect.pivot = new Vector2(0.5f, 0.95f);
                break;

            case SimpleTextUnitModel.Anchor.MiddleCenter:
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.pivot = new Vector2(0.5f, 0.5f);
                break;

            case SimpleTextUnitModel.Anchor.BottomCenter:
                rect.anchorMin = new Vector2(0.5f, 0.05f);
                rect.anchorMax = new Vector2(0.5f, 0.05f);
                rect.pivot = new Vector2(0.5f, 0f);
                break;
            default:
                break;
        }

        rect.anchoredPosition = new Vector2(0, 0);
    }

    public override void Refresh()
    {
        text.SafeSetText(Model.Text);
    }
}
