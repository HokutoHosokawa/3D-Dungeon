using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    private readonly (int spawnWeight, string enemyName)[] EnemyNames = {(1, "Zombie")};
    private readonly int MinEnemyCount = 5;
    private readonly int MaxEnemyCount = 15;
    private readonly int MinEnemySpawnRange = 10;
    private readonly int MaxEnemySpawnRange = 30;
    private readonly float SpawnProbability = 0.0003f;
    private readonly int EnemyLevelDiffFromPlayer = 1;
    private readonly int MinLevel = 1;

    private int EnemyTypes;
    private int SumSpawnWeight;
    private CreateFloor _createFloor;
    private GameObject _player;
    private Dictionary<string, (int spawnWeight, EnemyController enemyController)> _enemyControllers = new Dictionary<string, (int, EnemyController)>();
    private int _enemyCount = 0;
    private int[,] _map;

    // Start is called before the first frame update
    void Start()
    {
        _createFloor = GameObject.Find("CreateDungeonObject").GetComponent<CreateFloor>();
        _map = _createFloor.GetCurrentFloorManagement().CreateDungeon.Map;
        EnemyTypes = EnemyNames.Length;
        _player = GameObject.FindGameObjectWithTag("Player");
        SumSpawnWeight = InitializeEnemyControllers();
    }

    // Update is called once per frame
    void Update()
    {
        _enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
        if(_enemyCount >= MaxEnemyCount)
        {
            return;
        }
        List<Vector3Int> spawnablePositions = SetSpawnablePositions();
        int spawnablePositionCount = spawnablePositions.Count;
        while(_enemyCount < MinEnemyCount)
        {
            SpawnEnemy(spawnablePositions, spawnablePositionCount);
            return;
        }
        if(Random.Range(0f, 1f) < SpawnProbability)
        {
            SpawnEnemy(spawnablePositions, spawnablePositionCount);
        }
    }

    private int InitializeEnemyControllers()
    {
        int sumWeight = 0;
        for(int i = 0; i < EnemyTypes; i++)
        {
            sumWeight += EnemyNames[i].spawnWeight;
        }
        for (int i = 0; i < EnemyTypes; i++)
        {
            _enemyControllers.Add(EnemyNames[i].enemyName, (EnemyNames[i].spawnWeight, new EnemyController("EntityData/" + EnemyNames[i].enemyName)));
        }
        return sumWeight;
    }

    private List<Vector3Int> SetSpawnablePositions()
    {
        Vector3 spawnPositionCenter = _player.transform.position;
        Vector3Int spawnPositionCenterInt = new Vector3Int(Mathf.FloorToInt((spawnPositionCenter.x + (CommonConst.FloorWidth / 2.0f)) / CommonConst.FloorWidth), 0, Mathf.FloorToInt((spawnPositionCenter.z + (CommonConst.FloorHeight / 2.0f)) / CommonConst.FloorHeight));
        // プレイヤー周りでMinEnemySpawnRange以上MaxEnemySpawnRange以下の範囲で敵をスポーンさせる
        // 最初にマップ情報と照らし合わせ、スポーン可能な座標をリストアップする
        List<Vector3Int> spawnablePositions = new List<Vector3Int>();
        for(int i = -MaxEnemySpawnRange; i <= MaxEnemySpawnRange; i++)
        {
            for(int j = -MaxEnemySpawnRange; j <= MaxEnemySpawnRange; j++)
            {
                Vector3Int spawnPosition = new Vector3Int(spawnPositionCenterInt.x + j, 0, spawnPositionCenterInt.z + i);
                if(spawnPosition.x < 0 || spawnPosition.x >= CommonConst.MapWidth || spawnPosition.z < 0 || spawnPosition.z >= CommonConst.MapHeight)
                {
                    continue;
                }
                float distance = Vector3Int.Distance(spawnPositionCenterInt, spawnPosition);
                if(distance < MinEnemySpawnRange || distance > MaxEnemySpawnRange)
                {
                    continue;
                }
                if(_map[spawnPosition.z, spawnPosition.x] == CommonConst.DungeonArea)
                {
                    spawnablePositions.Add(spawnPosition);
                }
            }
        }
        return spawnablePositions;
    }

    private void SpawnEnemy(IReadOnlyList<Vector3Int> spawnablePositions, int spawnablePositionCount)
    {
        if(spawnablePositionCount == 0)
        {
            return;
        }
        Vector3Int spawnPositionInt = spawnablePositions[Random.Range(0, spawnablePositionCount)];
        Vector3 spawnPosition = new Vector3(spawnPositionInt.x * CommonConst.FloorWidth, CommonConst.FloorThickness / 2.0f, spawnPositionInt.z * CommonConst.FloorHeight);
        // 敵の種類を決定する
        int randomValue = Random.Range(0, SumSpawnWeight);
        int sumWeight = 0;
        for(int i = 0; i < EnemyTypes; ++i)
        {
            sumWeight += _enemyControllers[EnemyNames[i].enemyName].spawnWeight;
            if(randomValue < sumWeight)
            {
                string enemyName = EnemyNames[i].enemyName;
                int levelDiffFromPlayer = Random.Range(-EnemyLevelDiffFromPlayer, EnemyLevelDiffFromPlayer + 1);
                // プレイヤーのレベル処理がまだできていない
                int playerLevel = _player.GetComponent<PlayerStatus>().PlayerInfo.Level;
                int enemyLevel = playerLevel + levelDiffFromPlayer;
                int maxLevel = _enemyControllers[enemyName].enemyController.GetMaxLevel();
                if(enemyLevel < MinLevel)
                {
                    enemyLevel = MinLevel;
                }
                if(enemyLevel > maxLevel)
                {
                    enemyLevel = maxLevel;
                }
                NavMeshHit hit;
                GameObject enemy;
                if(NavMesh.SamplePosition(spawnPosition, out hit, 50f, NavMesh.AllAreas))
                {
                    enemy = Instantiate(Resources.Load("Prefabs/Enemy/" + enemyName), hit.position, Quaternion.identity) as GameObject;
                    enemy.GetComponent<EnemyStatus>().Initialize(new Enemy(_enemyControllers[enemyName].enemyController.GetStatus(enemyLevel), enemy));
                    return;
                }
                else
                {
                    Debug.LogError("NavMeshのサンプリングに失敗しました");
                }
                return;
            }
        }
    }
}
