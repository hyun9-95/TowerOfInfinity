using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

public class CheatManager : BaseMonoManager<CheatManager>
{
    private void Awake()
    {
#if CHEAT
        instance = this;
        SetState(State.Collapse);

#if !UNITY_EDITOR
        drawCallText.gameObject.SafeSetActive(false);
#endif

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

    [SerializeField]
    private TextMeshProUGUI fpsText;

    [SerializeField]
    private TextMeshProUGUI memoryText;

    [SerializeField]
    private TextMeshProUGUI drawCallText;

    [SerializeField]
    private Toggle toggleInvincible;

    [SerializeField]
    private Toggle toggleExpBoost;

    [SerializeField]
    private Toggle toggleWaveBoost;

    private CheatConfig cheatConfig;
    
    private float deltaTime = 0f;
    private float updateInterval = 0.5f;
    private float lastUpdateTime = 0f;
    #endregion

    #region Function
    private void OnEnable()
    {
    }

    private void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        
        if (Time.time - lastUpdateTime >= updateInterval)
        {
            UpdatePerformanceDisplay();
            lastUpdateTime = Time.time;
        }
    }
    
    private void UpdatePerformanceDisplay()
    {
        UpdateFPS();
        UpdateMemoryUsage();

#if UNITY_EDITOR
        UpdateDrawCalls();
#endif
    }
    
    private void UpdateFPS()
    {
        if (fpsText == null)
            return;
            
        float fps = 1.0f / deltaTime;
        var sb = GlobalStringBuilder.Get();
        sb.Append(fps.ToString("0"));
        sb.Append(" fps");
        fpsText.SafeSetText(sb.ToString());
    }
    
    private void UpdateMemoryUsage()
    {
        if (memoryText == null)
            return;
            
        long totalMemory = Profiler.GetTotalAllocatedMemoryLong();
        long reservedMemory = Profiler.GetTotalReservedMemoryLong();
        
        float usedMemoryMB = totalMemory / (1024f * 1024f);
        float usagePercentage = (float)totalMemory / reservedMemory * 100f;
        
        var sb = GlobalStringBuilder.Get();
        sb.Append(usedMemoryMB.ToString("F0"));
        sb.Append("mb (");
        sb.Append(usagePercentage.ToString("F0"));
        sb.Append("%)");
        memoryText.SafeSetText(sb.ToString());
    }
    
    private void UpdateDrawCalls()
    {
        if (drawCallText == null)
            return;
            
#if UNITY_EDITOR
        int batches = UnityEditor.UnityStats.batches;
        int dynBatched = UnityEditor.UnityStats.dynamicBatchedDrawCalls;
        int statBatched = UnityEditor.UnityStats.staticBatchedDrawCalls;
        int instBatched = UnityEditor.UnityStats.instancedBatchedDrawCalls;
        int saved = dynBatched + statBatched + instBatched;
        var sb = GlobalStringBuilder.Get();
        sb.Append("b:");
        sb.Append(batches.ToString());
        sb.Append(" s:");
        sb.Append(saved.ToString());
        drawCallText.SafeSetText(sb.ToString());
#endif
    }

    public void SetCheatConfig(CheatConfig config)
    {
        cheatConfig = config;

        if (cheatConfig.enableGcAllocCallStack)
            Profiler.enableAllocationCallstacks = true;

        toggleInvincible.isOn = cheatConfig.ToggleInvincible;
        toggleExpBoost.isOn = cheatConfig.ToggleExpBoostX2;
        toggleWaveBoost.isOn = cheatConfig.ToggleWaveBoostX2;

#if !UNITY_EDITOR
        memoryText.gameObject.SafeSetActive(false);
        drawCallText.gameObject.SafeSetActive(false);
#endif
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
        BattleSystemManager.instance.CheatLevelUp();
    }

    public static void BattleSpawnBoss()
    {
        BattleSceneManager.Instance.CheatSpawnBoss();
    }

    public static void OnToggleInvincible(bool value)
    {
        // True일 경우 받는 피해 0
        CheatConfig.ToggleInvincible = value;
    }

    public static void OnToggleExpBoost(bool value)
    {
        // True일 경우 경험치 획득량 2배
        CheatConfig.ToggleExpBoostX2 = value;
    }

    public static void OnToggleWaveBoost(bool value)
    {
        // True일 경우 웨이브 증가 요구 시간 2분의 1로 감소
        CheatConfig.ToggleWaveBoostX2 = value;
    }

    public static void BattleAddWave()
    {
        BattleSystemManager.instance.CheatAddWave();
    }

    public static void BattleDrawCard(int tierInt)
    {
        BattleCardTier tier = (BattleCardTier)tierInt;
        BattleSystemManager.instance.CheatLevelUpWithDraw(tier);
    }

    #endregion
    #endregion
#endif
    }