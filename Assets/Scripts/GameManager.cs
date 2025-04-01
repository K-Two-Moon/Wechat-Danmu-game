using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    Player player;

    private float lastSpawnTime = 0f;
    private float moveSpeedEnemy = 5f; // 敌人和 Buff 的移动速度
    private List<GameObject> spawnedObjects = new List<GameObject>(); // 存储已生成的对象
    public List<EnemoyBase> enemyList = new List<EnemoyBase>(); // 存储所有敌人

    /// <summary>
    /// 游戏是否结束
    /// </summary>
    bool isGameRunning = false;

    /// <summary>
    /// 怪物刷新次数
    /// </summary>
    [Header("怪物刷新次数")] [SerializeField] private int enemyRefreshCount;

    void Start()
    {
        Application.targetFrameRate = 30;

        GameObject.Find("Plane").GetComponent<MeshRenderer>().material.color = Color.black;


        InitEvent();
        InitObject();
    }

    private void InitEvent()
    {
        MessageManager.AddListener(CMD.EVENT_RESTART_GAME, InitObject);
        MessageManager.AddListener(CMD.EVENT_GAME_WIN, GameWin);
        MessageManager.AddListener(CMD.EVENT_GAME_LOSE, GameLose);
    }

    private void OnDestroy()
    {
        MessageManager.RemoveListener(CMD.EVENT_RESTART_GAME, InitObject);
        MessageManager.RemoveListener(CMD.EVENT_GAME_WIN, GameWin);
        MessageManager.RemoveListener(CMD.EVENT_GAME_LOSE, GameLose);
    }

    private void GameWin()
    {
        isGameRunning = false;
        foreach (var enemy in spawnedObjects)
        {
            Destroy(enemy);
        }

        foreach (var enemy in enemyList)
        {
            enemy.Destroy();
        }

        spawnedObjects.Clear();
        enemyList.Clear();
        player.Destroy();
        enemyRefreshCount = 5;
    }

    private void GameLose()
    {
        isGameRunning = false;
        foreach (var enemy in spawnedObjects)
        {
            Destroy(enemy);
        }

        foreach (var enemy in enemyList)
        {
            enemy.Destroy();
        }

        spawnedObjects.Clear();
        enemyList.Clear();
        player.Destroy();
        enemyRefreshCount = 5;
        BulletData.Reset();
    }


    private void InitObject()
    {
        player = new Player();

        // 预先生成敌人和 Buff还有Boss
        SpawnTime();
    }

    /// <summary>
    /// 每隔 5 秒生成一个敌人和一个 Buff，并将生成的对象存储在 spawnedObjects 列表中。
    /// </summary>
    async private void SpawnTime()
    {
        isGameRunning = true;
        while (Application.isPlaying && enemyRefreshCount > 0)
        {
            if (isGameRunning == false) return;
            enemyRefreshCount--;
            SpawnEnemyAndBuff();
            await Task.Delay(4000);
        }

        if (isGameRunning == false) return;
        if (Application.isPlaying)
        {
            //最后刷新Boss
            Boss boss = new Boss();
            boss.Init();
            enemyList.Add(boss);
        }
    }


    void Update()
    {
        player.Update();
        // 统一更新所有生成对象的位置
        UpdateSpawnedObjects();
    }

    /// <summary>
    /// 同时生成一个敌人和一个 Buff，两者分别占用两个预设位置，确保不会重叠。
    /// </summary>
    private void SpawnEnemyAndBuff()
    {
        // 两个预设位置
        Vector3 leftPos = new Vector3(-2.5f, 0, 25);
        Vector3 rightPos = new Vector3(2.5f, 0, 25);

        // 随机决定哪个位置生成敌人，另一个生成 Buff
        bool enemyAtLeft = Random.Range(0, 2) == 0;
        if (enemyAtLeft)
        {
            SpawnEnemy(leftPos);
            SpawnBuff(rightPos);
        }
        else
        {
            SpawnEnemy(rightPos);
            SpawnBuff(leftPos);
        }
    }

    private void SpawnEnemy(Vector3 position)
    {
        GameObject enemy = Instantiate(Resources.Load<GameObject>("Enemy"));
        enemy.AddComponent<Enemy>().Init(position);
        spawnedObjects.Add(enemy);
    }

    private void SpawnBuff(Vector3 position)
    {
        GameObject buff = Instantiate(Resources.Load<GameObject>("Buff"));
        buff.AddComponent<Buff>().Init(position);
        spawnedObjects.Add(buff);
    }

    /// <summary>
    /// 更新所有生成对象的位置，统一以 (0, 0, -1) 方向移动。
    /// </summary>
    private void UpdateSpawnedObjects()
    {
        foreach (var obj in spawnedObjects)
        {
            if (obj != null)
            {
                obj.transform.Translate(Vector3.back * moveSpeedEnemy * Time.deltaTime, Space.World);
            }
        }

        foreach (var enemy in enemyList)
        {
            if (enemy != null)
            {
                enemy.Update(moveSpeedEnemy);
            }
        }
    }
}