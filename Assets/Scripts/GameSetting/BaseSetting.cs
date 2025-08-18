using UnityEngine;

public abstract class BaseSetting
{
    public string UserId { get; private set; }

    public void SetPrefsKey(string id)
    {
        UserId = id;
    }

    public virtual void Load() {}

    public string GetKey(string format)
    {
        return string.Format(format, UserId);
    }
}
