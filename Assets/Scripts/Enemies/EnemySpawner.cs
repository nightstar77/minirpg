using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敌人刷新器：按配置在指定位置周期性刷新敌人，维持场景中的敌人数量。
/// 挂在场景中的空物体上，Inspector 拖入敌人预制体与刷新点即可，无需改代码。
///
/// 存活判定：死亡的外敌会自行 Destroy（EnemyBase.deadEnter 延迟销毁），
/// 这里每帧清理已销毁的引用，从而统计真实存活数量。
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    #region ===== Inspector =====
    [Header("敌人预制体（随机选择，可放多个）")]
    [SerializeField]
    private GameObject[] enemyPrefabs;

    [Header("刷新位置")]
    [Tooltip("刷新点；留空则使用本物体所在位置")]
    [SerializeField]
    private Transform spawnPoint;

    [Tooltip("水平方向随机偏移范围（米），让敌人不要全部叠在同一点")]
    [SerializeField]
    private float horizontalSpread = 2f;

    [Header("刷新节奏")]
    [Tooltip("同时在场的敌人上限")]
    [SerializeField]
    private int maxAlive = 4;

    [Tooltip("两次刷新之间的间隔（秒）")]
    [SerializeField]
    private float spawnInterval = 3f;

    [Tooltip("进入场景后首次刷新的延迟（秒）")]
    [SerializeField]
    private float initialDelay = 1f;

    [Tooltip("勾选：无限刷新（适合刷怪点）；不勾选：总共只刷新 totalSpawnCount 个（适合波次）")]
    [SerializeField]
    private bool infinite = true;

    [Tooltip("不勾选 infinite 时生效：总共刷新多少个敌人")]
    [SerializeField]
    private int totalSpawnCount = 10;
    #endregion

    #region ===== Runtime =====
    private readonly List<GameObject> aliveEnemies = new List<GameObject>();
    private float spawnTimer;
    private int spawnedCount;
    private bool finished;   // 有限刷新模式下，达到总量后停止
    #endregion

    private void Start()
    {
        spawnTimer = initialDelay;
    }

    private void Update()
    {
        PruneDeadEnemies();

        if (finished)
        {
            return;
        }

        // 暂停时 Time.deltaTime 为 0，计时自然停止
        spawnTimer -= Time.deltaTime;
        if (spawnTimer > 0f)
        {
            return;
        }

        // 场上敌人已满，稍后再检查（避免每帧重复判断）
        if (aliveEnemies.Count >= maxAlive)
        {
            spawnTimer = 0.5f;
            return;
        }

        // 有限模式下已刷满总量
        if (!infinite && spawnedCount >= totalSpawnCount)
        {
            finished = true;
            return;
        }

        SpawnOne();
        spawnTimer = spawnInterval;
    }

    /// <summary>
    /// 清理已销毁（已死亡）的敌人引用
    /// </summary>
    private void PruneDeadEnemies()
    {
        for (int i = aliveEnemies.Count - 1; i >= 0; i--)
        {
            if (aliveEnemies[i] == null)
            {
                aliveEnemies.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// 刷新一个敌人
    /// </summary>
    private void SpawnOne()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            return;
        }

        GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        if (prefab == null)
        {
            return;
        }

        Transform point = spawnPoint != null ? spawnPoint : transform;
        Vector3 pos = point.position + new Vector3(
            Random.Range(-horizontalSpread, horizontalSpread), 0f, 0f);

        GameObject enemy = Instantiate(prefab, pos, Quaternion.identity);
        aliveEnemies.Add(enemy);
        spawnedCount++;

        Debug.Log($"[EnemySpawner] 刷新敌人 {prefab.name}（第 {spawnedCount} 个，场上 {aliveEnemies.Count}）");
    }
}
