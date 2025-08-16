#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BattleEnemySpawner : IObserver
{
    public BattleEnemyGeneratorModel Model;

    private float minDistance;
    private int safeCount = 3;

    private float[] currentWaveWeights;
    private CharacterDefine[] currentWaveEnemies;
    private float totalWeight;
    bool isSpawnMidBoss = false;
    bool isSpawnFinalBoss = false;

    public BattleEnemySpawner(BattleEnemyGeneratorModel battleEnemyGeneratorModel)
    {
        Model = battleEnemyGeneratorModel;
    }

    public async UniTask StartGenerateAsync()
    {
        // 카메라 영역에서 벗어나기 위한 중심점으로부터의 최소 거리
        minDistance = CameraManager.Instance.DiagonalLengthFromCenter;

        ObserverManager.AddObserver(BattleObserverID.BattleEnd, this);

        int currentWave = -1;
        int burstSpawnCount = 1;
        bool isBurstSpawn = false;
        float currentIntervalSeconds = Model.SpawnIntervalSeconds;

#if CHEAT && UNITY_EDITOR
        if (CheatManager.CheatConfig.bossSpawnWhenBattleStart)
            await SpawnBossAsync();

        if (CheatManager.CheatConfig.midBossSpawnWhenBattleStart)
            await SpawnMidBossAsync();
#endif

        while (BattleSystemManager.InBattle)
        {
            var tempWave = BattleSystemManager.Instance.CurrentWave;

            if (tempWave != currentWave)
            {
                currentWave = tempWave;
                currentIntervalSeconds = Model.GetCurrentSpawnInterval(currentWave);
                currentWaveEnemies = Model.GetCurrentWaveEnemies(currentWave);
                CachingCurrentWaveWeight(currentWaveEnemies, currentWave);
                burstSpawnCount = Model.GetBurstWaveValue(currentWave);
                isBurstSpawn = false;
            }

            if (!isSpawnMidBoss && currentWave == Model.MidBossWave)
            {
                await SpawnMidBossAsync();
                isSpawnMidBoss = true;
            }

            if (!isSpawnFinalBoss && currentWave == Model.FinalBossWave)
            {
                await SpawnBossAsync();
                isSpawnFinalBoss = true;
            }

            if (!isSpawnFinalBoss && currentWaveWeights != null)
            {
                // 특정 웨이브 도달 시 대량 스폰 1회
                if (!isBurstSpawn && burstSpawnCount > 0)
                {
                    var tasks = new List<UniTask>(burstSpawnCount);
                    for (int i = 0; i < burstSpawnCount; i++)
                    {
                        var enemy = PickRandomEnemy();
                        tasks.Add(SpawnWave(enemy));
                    }
                    await UniTask.WhenAll(tasks);

                    isBurstSpawn = true;
                }
                else
                {
                    var enemy = PickRandomEnemy();
                    await SpawnWave(enemy);
                }
            }

            await UniTaskUtils.DelaySeconds(currentIntervalSeconds, cancellationToken: TokenPool.Get(GetHashCode()));
        }

        ObserverManager.RemoveObserver(BattleObserverID.BattleEnd, this);
    }

    // 웨이브의 적 종류별 가중치를 캐싱해놓는다.
    private void CachingCurrentWaveWeight(CharacterDefine[] enemies, int currentWave)
    {
        currentWaveWeights = new float[enemies.Length];
        currentWaveEnemies = enemies;
        totalWeight = 0f;
        var weightSum = 0f;

        for (int i = 0; i < enemies.Length; i++)
        {
            var enemy = enemies[i];
            var enemyWeight = Model.OnGetEnemySpawnWeight(enemy, currentWave);

            weightSum += enemyWeight;
            currentWaveWeights[i] = weightSum;
        }

        totalWeight = weightSum;
    }

    // 캐싱한 가중치 배열에서 적을 랜덤으로 선택한다.
    private CharacterDefine PickRandomEnemy()
    {
        float randomValue = UnityEngine.Random.value * totalWeight;

        int selectIndex = 0;
        int highIndex = currentWaveWeights.Length - 1;

        while (selectIndex <= highIndex)
        {
            int midIndex = (selectIndex + highIndex) / 2;

            if (currentWaveWeights[midIndex] < randomValue)
                selectIndex = midIndex + 1;
            else
                highIndex = midIndex - 1;
        }

        return currentWaveEnemies[selectIndex];
    }

    private async UniTask SpawnMidBossAsync()
    {
        await SpawnWave(Model.MidBoss, true);
        isSpawnMidBoss = true;
        Logger.Log($"Spawn MidBoss : {Model.MidBoss}");
    }

    private async UniTask SpawnBossAsync()
    {
        await SpawnWave(Model.FinalBoss, true);
        isSpawnFinalBoss = true;
        Logger.Log($"Spawn FinalBoss : {Model.FinalBoss}");
    }

#if CHEAT
    public void CheatSpawnBoss()
    {
        SpawnBossAsync().Forget();
    }
#endif

    private async UniTask SpawnWave(CharacterDefine characterDefine, bool spawnBoss = false)
    {
        var spawnPos = GetValidSpawnPosition();

        if (spawnPos == Vector3.zero)
        {
            int tryCount = 0;

            while (spawnPos == Vector3.zero && tryCount < safeCount)
            {
                await UniTaskUtils.NextFrame(cancellationToken: TokenPool.Get(GetHashCode()));
                spawnPos = GetValidSpawnPosition();
                tryCount++;
            }

            if (spawnPos == Vector3.zero)
                return;
        }

        var enemy = spawnBoss ?
            await CharacterFactory.Instance.SpawnBoss((int)characterDefine, spawnPos):
            await CharacterFactory.Instance.SpawnEnemy((int)characterDefine, spawnPos);

        if (enemy)
            Model.OnSpawnEnemy(enemy);
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

    void IObserver.HandleMessage(System.Enum observerMessage, IObserverParam observerParam)
    {
        if (observerParam is not BattleObserverParam)
            return;

        BattleObserverParam param = (BattleObserverParam)observerParam;

        switch (observerMessage)
        {
            case BattleObserverID.BattleEnd:
                Cancel();
                break;
        }
    }
}
