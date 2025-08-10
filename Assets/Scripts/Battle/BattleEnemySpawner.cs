#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BattleEnemySpawner
{
    public BattleEnemyGeneratorModel Model;

    private float minDistance;
    private int safeCount = 3;

    public BattleEnemySpawner(BattleEnemyGeneratorModel battleEnemyGeneratorModel)
    {
        Model = battleEnemyGeneratorModel;
    }

    public async UniTask StartGenerateAsync()
    {
        // 카메라 영역에서 벗어나기 위한 중심점으로부터의 최소 거리
        minDistance = CameraManager.Instance.DiagonalLengthFromCenter;

        while (!TokenPool.Get(GetHashCode()).IsCancellationRequested)
        {
            int currentWave = BattleSystemManager.Instance.CurrentWave;
            var currentWaveEnemies = Model.GetCurrentWaveEnemies(currentWave);

            if (currentWaveEnemies != null)
                await SpawnWave(currentWaveEnemies);

            await UniTaskUtils.DelaySeconds(Model.SpawnIntervalSeconds, cancellationToken: TokenPool.Get(GetHashCode()));
        }
    }

    private async UniTask SpawnWave(CharacterDefine[] enemys)
    {
        var characterContainer = DataManager.Instance.GetDataContainer<DataCharacter>();

        foreach (var characterDefine in enemys)
        {
            var spawnPos = GetValidSpawnPosition();

            if (spawnPos == Vector3.zero)
            {
                int tryCount = 0;

                while (spawnPos != Vector3.zero && tryCount < safeCount)
                {
                    spawnPos = GetValidSpawnPosition();
                    tryCount++;
                }

                if (spawnPos == Vector3.zero)
                    continue;
            }

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
    public Vector3 GetValidSpawnPosition()
    {
        Vector3 cameraCenter = CameraManager.Instance.GetBrainOutputPosition();

        // 랜덤 각도
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

        // 랜덤 거리 (카메라 범위에서 벗어난 거리 + offset)
        float offset = 0.5f;
        float distance = Random.Range(minDistance, minDistance + offset);

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
