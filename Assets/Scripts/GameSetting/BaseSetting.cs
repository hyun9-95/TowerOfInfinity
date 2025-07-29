using UnityEngine;

public abstract class BaseSetting
{
    public virtual void Load() {}

    public string GetKey(string format)
    {
        return string.Format(format, PlayerManager.Instance.MyUser.ID);
    }
}
