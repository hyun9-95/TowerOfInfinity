using UnityEngine;

[System.Serializable]
public class Config 
{
    [Header("전투로 바로 진입")]
    public bool IsEnterBattleDirectly;

    [Header("커스터마이징 플로우 진입")]
    public bool IsEnterCustomizationFlow;

    [Header("Walkable Node, 경로 표시")]
    public bool IsDebugAStar;
}
