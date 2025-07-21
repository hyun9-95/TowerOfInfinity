#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BattleEnemyGenerator
{
    public BattleEnemyGeneratorModel Model;

    public BattleEnemyGenerator(BattleEnemyGeneratorModel battleEnemyGeneratorModel)
    {
        Model = battleEnemyGeneratorModel;
    }

    public async UniTask StartGenerateAsync()
    {
        var camera = CameraManager.Instance.GetWorldCamera();

        while (!TokenPool.Get(GetHashCode()).IsCancellationRequested)
        {
            var spawnPos = GetValidSpawnPosition(camera);

            if (spawnPos == Vector3.zero)
                continue;

            var currentWave = Model.GetCurrentWave();

            if (currentWave != null)
                await SpawnWave(spawnPos, Model.GetCurrentWave());

            await UniTaskUtils.DelaySeconds(Model.SpawnIntervalSeconds, cancellationToken: TokenPool.Get(GetHashCode()));
        }
    }

    private async UniTask SpawnWave(Vector3 spawnPos, CharacterDefine[] enemys)
    {
        var characterContainer = DataManager.Instance.GetDataContainer<DataCharacter>();

        foreach (var characterDefine in enemys)
        {
            var enemy = await CharacterFactory.Instance.SpawnEnemy((int)characterDefine, spawnPos);

            if (enemy)
                Model.OnSpawnEnemy(enemy);
        }
    }

    public void Cancel()
    {
        TokenPool.Cancel(GetHashCode());
    }

    /// <summary>
    /// 카메라 영역에서 보이지 않는 랜덤 생성 포지션
    /// </summary>
    public Vector3 GetValidSpawnPosition(Camera camera)
    {
        Vector3 cameraCenter = CameraManager.Instance.GetBrainOutputPosition();
        Vector3 bottomLeft = camera.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 topRight = camera.ViewportToWorldPoint(new Vector3(1, 1, 0));

        float camHalfWidth = Mathf.Abs(topRight.x - bottomLeft.x) / 2f;
        float camHalfHeight = Mathf.Abs(topRight.y - bottomLeft.y) / 2f;

        // 대각선 거리 = 카메라에서 보이는 최소 거리
        float minDistance = Mathf.Sqrt(camHalfWidth * camHalfWidth + camHalfHeight * camHalfHeight);

        //랜덤 각도
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

        //랜덤 거리
        float distance = Random.Range(minDistance, minDistance + 1f);

        Vector3 validPos = cameraCenter + (Vector3)(direction * distance);
        validPos.z = 0;

        // AStar를 사용중이라면, 현재 위치가 걸을 수 있는 위치인지 체크하고 반환한다.
        if (Model.CheckWalkablePosOnSpawn)
        {
            if (AStarManager.Instance.IsWalkablePos(validPos))
                return validPos;
        }
        else
        {
            // 사용 중 아니면 그냥 반환
            return validPos;
        }

        // 걸을 수 없다면, 다음 프레임에 재시도한다.
        return Vector3.zero;
    }
}
