using Cysharp.Threading.Tasks;
using UnityEngine;

public class CheatManager : BaseMonoManager<CheatManager>
{
    private void Awake()
    {
#if CHEAT
        instance = this;
        SetState(State.Collapse);
#else
        Destroy(gameObject);
#endif
    }

#if CHEAT
    public enum State
    {
        Collapse,
        Expand,
    }

    #region Property
    public static CheatConfig CheatConfig => Instance.cheatConfig;

    #endregion

    #region Value
    [SerializeField]
    private GameObject[] stateObjects;

    [SerializeField]
    private GameObject expandObject;

    private CheatConfig cheatConfig;
    #endregion

    #region Function
    public void SetCheatConfig(CheatConfig config)
    {
        cheatConfig = config;
    }

    public void SetState(State state)
    {
        int index = (int)state;

        for (int i = 0; i < stateObjects.Length; i++)
            stateObjects[i].SetActive(i == index);

        expandObject.SetActive(state == State.Expand);
    }

    private async UniTask DrawWireSphereCoroutine(Vector3 center, float radius)
    {
        float startTime = Time.time;

        while (Time.time - startTime < 1f)
        {
            DrawWireSphereGizmo(center, radius);
            await UniTask.NextFrame();
        }
    }

    private void DrawWireSphereGizmo(Vector3 center, float radius)
    {
        int segments = 32;
        float angleStep = 360f / segments;

        for (int i = 0; i < segments; i++)
        {
            float angle1 = i * angleStep * Mathf.Deg2Rad;
            float angle2 = (i + 1) * angleStep * Mathf.Deg2Rad;

            Vector3 point1 = center + new Vector3(Mathf.Cos(angle1), Mathf.Sin(angle1), 0) * radius;
            Vector3 point2 = center + new Vector3(Mathf.Cos(angle2), Mathf.Sin(angle2), 0) * radius;

            Debug.DrawLine(point1, point2, Color.yellow, 0.1f);
        }
    }

    #region CHEAT
    public static void OnCheatStateChange(int state)
    {
        Instance.SetState((State)state);
    }

    public static void DrawWireSphereFromMainCharacter(float radius)
    {
        var mainPlayerCharacter = PlayerManager.Instance.GetMainPlayerCharacter();
        if (mainPlayerCharacter == null)
            return;

        Vector3 center = mainPlayerCharacter.CharacterUnit.transform.position;
        DrawWireSphereAtPosition(center, radius);
    }

    public static void DrawWireSphereAtPosition(Vector3 center, float radius)
    {
        Instance.DrawWireSphereCoroutine(center, radius).Forget();
    }

    public static void BattleLevelUp()
    {
        if (FlowManager.Instance.CurrentFlowType != FlowType.BattleFlow)
            return;

        if (BattleSystemManager.Instance == null)
            return;

        if (UIManager.instance.CurrentOpenUI == UIType.BattleCardSelectPopup)
            return;

        var battleInfo = BattleSystemManager.Instance.BattleInfo;
        var nextExp = battleInfo.NextBattleExp;
        var currentExp = battleInfo.BattleExp;
        var diff = nextExp - currentExp;

        BattleSystemManager.instance.CheatLevelUp(currentExp + diff);
    }
    #endregion
    #endregion
#endif
}