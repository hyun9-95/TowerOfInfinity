using UnityEngine;

[System.Serializable]
public class Config 
{
    [Header("전투로 바로 진입")]
    public bool IsEnterBattleDirectly;

    [Header("Walkable Node, 경로 표시")]
    public bool IsDebugAStar;
}
