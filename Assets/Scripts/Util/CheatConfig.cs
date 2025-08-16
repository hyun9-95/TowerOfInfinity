using UnityEngine;

[System.Serializable]
public class CheatConfig 
{
    public bool IsEnterBattleDirectly;

    public AbilityDefine[] cheatAbility;

    public bool IsEnterCustomizationFlow;

    public bool IsDebugAStar;

    public bool IsInvincible;

    public SceneDefine SceneDefine;

    public bool enableGcAllocCallStack = false;

    public LocalizationType testLocalType;
}
