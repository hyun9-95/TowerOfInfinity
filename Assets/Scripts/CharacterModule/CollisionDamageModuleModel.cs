using System.Collections.Generic;
using UnityEngine;

public class CollisionDamageModuleModel : IModuleModel
{
    public Dictionary<GameObject, CollisionDamageInfo> CollisionEnterTargetTimer = new Dictionary<GameObject, CollisionDamageInfo>();
}
