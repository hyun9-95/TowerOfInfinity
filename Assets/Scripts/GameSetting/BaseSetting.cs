using UnityEngine;

public abstract class BaseSetting
{
    public int UserId { get; private set; }

    public void SetPrefsKey(int id)
    {
        UserId = id;
    }

    public virtual void Load() {}

    public string GetKey(string format)
    {
        return string.Format(format, UserId);
    }
}
