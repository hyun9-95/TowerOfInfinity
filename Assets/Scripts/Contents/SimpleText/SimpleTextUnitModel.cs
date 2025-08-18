using System;

public class SimpleTextUnitModel : IBaseUnitModel
{
    public enum Anchor
    {
        TopCenter,
        MiddleCenter,
        BottomCenter,
    }

    public string Text { get; private set; }

    public Anchor TextAnchor { get; private set; }

    public Action<string> OnUpdateText { get; private set; }

    public void SetText(string value)
    {
        Text = value;
    }

    public void SetAnchor(Anchor anchor)
    {
        TextAnchor = anchor;
    }

    public void SetOnUpdateText(Action<string> value)
    {
        OnUpdateText = value;
    }
}
