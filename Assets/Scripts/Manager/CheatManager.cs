using Cysharp.Threading.Tasks;
using UnityEngine;

public class CheatManager : BaseMonoManager<CheatManager>
{
    public static CheatConfig CheatConfig => Instance.cheatConfig;

    [SerializeField]
    private CheatConfig cheatConfig;

    private void Awake()
    {
        instance = this;
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
}