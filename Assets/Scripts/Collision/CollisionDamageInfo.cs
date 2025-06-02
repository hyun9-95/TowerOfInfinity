using UnityEngine;

public class CollisionDamageInfo
{
    public float timer;
    public CharacterUnitModel targetModel;

    public CollisionDamageInfo(float timer, CharacterUnitModel targetModel)
    {
        this.timer = timer;
        this.targetModel = targetModel;
    }
}
