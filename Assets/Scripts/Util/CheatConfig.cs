using UnityEngine;

[System.Serializable]
public class CheatConfig 
{
    [Header("전투로 바로 진입")]
    public bool IsEnterBattleDirectly;

    [Header("전투에서 추가할 능력")]
    public AbilityDefine[] cheatAbility;

    [Header("커스터마이징 플로우 진입")]
    public bool IsEnterCustomizationFlow;

    [Header("Walkable Node + 경로 표시")]
    public bool IsDebugAStar;

    [Header("무적")]
    public bool IsInvincible;

    [Header("시작 마을 씬")]
    public SceneDefine SceneDefine;

    public bool enableGcAllocCallStack = false;

    public LocalizationType testLocalType;
}
