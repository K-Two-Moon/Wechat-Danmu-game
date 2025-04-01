using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Player
{
    Transform PlayerParent;
    public GameObject obj;
    private float moveSpeed = 5f; // 移动速度
    List<Transform> playerList = new List<Transform>();
    public static int Count = 0;

    public Player()
    {
        PlayerParent = new GameObject("PlayerParent").transform;
        PlayerParent.position = new Vector3(0, 0, -3);
        AddPlayer(1);

        InitEvent();
    }

    private void InitEvent()
    {
        //玩家撞到怪物后触发事件
        MessageManager.AddListener<Transform>(CMD.EVENT_DESTORY, RemovePlayerByEnemy);

        //子弹升级事件
        MessageManager.AddListener(CMD.EVENT_BULLET_UPGRADE, BulletDataUp);

        //玩家数量增加事件
        MessageManager.AddListener<int>(CMD.EVENT_ADD_PLAYER, AddPlayer);
    }
    
    public void Destroy()
    {
        //卸载所有事件
        MessageManager.RemoveListener<Transform>(CMD.EVENT_DESTORY, RemovePlayerByEnemy);
        MessageManager.RemoveListener(CMD.EVENT_BULLET_UPGRADE, BulletDataUp);
        MessageManager.RemoveListener<int>(CMD.EVENT_ADD_PLAYER, AddPlayer);
        
        //销毁所有玩家
        foreach (Transform player in PlayerParent.transform)
        {
            GameObject.Destroy(player.gameObject);
        }

        playerList.Clear();
        Count = playerList.Count;
    }

    private void RemovePlayerByEnemy(Transform target)
    {
        RemovePlayer(target);
    }

    private void BulletDataUp()
    {
        BulletData.LvUp();
    }

    private void Create()
    {
        obj = GameObject.Instantiate(Resources.Load<GameObject>("Player"));
        obj.name = "Player";
        obj.transform.SetParent(PlayerParent);
        obj.transform.position = Vector3.zero;
        //obj.GetComponent<MeshRenderer>().material.color = Color.cyan;
        //蒙皮骨骼绑定
        BindBone(obj);
        playerList.Add(obj.transform);


        Count++;
    }

    /// <summary>
    /// 绑定蒙皮骨骼
    /// </summary>
    private void BindBone(GameObject gameObject)
    {
        SkinnedMeshRenderer sk = gameObject.AddComponent<SkinnedMeshRenderer>();
        SkinnedMeshRenderer[] clothes = new SkinnedMeshRenderer[3];
        for (int i = 0; i < 3; i++)
        {
            clothes[i] = gameObject.transform.GetChild(i + 1).GetComponent<SkinnedMeshRenderer>();
        }

        Mesh mesh = new Mesh();
        CombineInstance[] instances = new CombineInstance[3];
        for (int i = 0; i < 3; i++)
        {
            instances[i].mesh = clothes[i].sharedMesh;
            instances[i].transform = clothes[i].transform.localToWorldMatrix;
        }

        mesh.CombineMeshes(instances, false, false);
        sk.sharedMesh = mesh;

        Material[] materials = new Material[3];
        for (int i = 0; i < 3; i++)
        {
            materials[i] = clothes[i].sharedMaterial;
        }

        sk.sharedMaterials = materials;

        Transform[] allBone = gameObject.GetComponentsInChildren<Transform>();
        Dictionary<string, Transform> boneDic = new Dictionary<string, Transform>();
        foreach (var bone in allBone)
        {
            boneDic.Add(bone.name, bone);
        }

        List<Transform> boneList = new List<Transform>();
        for (int i = 0; i < 3; i++)
        {
            SkinnedMeshRenderer cloth = clothes[i];
            for (int j = 0; j < cloth.bones.Length; j++)
            {
                boneList.Add(boneDic[cloth.bones[j].name]);
            }
        }

        sk.bones = boneList.ToArray();

        for (int i = 0; i < 3; i++)
        {
            GameObject.Destroy(clothes[i].gameObject);
        }
    }

    public void AddPlayer(int addCount)
    {
        //Debug.Log(addCount);
        if (addCount < 0) // 减少玩家
        {
            for (int i = 0; i > addCount; i--)
            {
                if (playerList.Count > 0)
                {
                    RemovePlayer(playerList[0]);
                }
            }
        }
        else if (addCount > 0) // 增加玩家
        {
            for (int i = 0; i < addCount; i++)
            {
                Create();
            }
        }
        else
        {
            return;
        }


        //调用调整队形位置方法
        AdjustPosition();
    }

    /// <summary>
    /// 调整队形位置（按实心圆排列）
    /// 所有点从中心开始，外圈点数取决于环的周长（理想点数 = Round(2π*ring)）。
    /// </summary>
    private void AdjustPosition()
    {
        int total = playerList.Count;
        if (total == 0) return;

        // 定义每圈的间距（也决定了圆的尺寸）
        float spacing = 0.5f;

        int index = 0;
        // 中心点
        playerList[index].localPosition = Vector3.zero;
        index++;

        int ring = 1; // 从第1圈开始
        while (index < total)
        {
            // 理想情况下，第 N 圈的点数为 Round(2π * N)
            int ringCount = Mathf.Max(Mathf.RoundToInt(2 * Mathf.PI * ring), 1);
            float ringRadius = ring * spacing;
            float angleStep = 2 * Mathf.PI / ringCount;

            for (int i = 0; i < ringCount && index < total; i++)
            {
                float angle = i * angleStep;
                float x = ringRadius * Mathf.Cos(angle);
                float z = ringRadius * Mathf.Sin(angle);
                playerList[index].localPosition = new Vector3(x, 0, z);
                index++;
            }

            ring++;
        }
    }


    private void Move()
    {
        float moveInput = 0;

        // 监听 A 和 D 键输入
        if (Input.GetKey(KeyCode.A))
        {
            moveInput = -1; // 向左移动
        }
        else if (Input.GetKey(KeyCode.D))
        {
            moveInput = 1; // 向右移动
        }

        // 计算新的 x 坐标
        float newX = PlayerParent.position.x + moveInput * moveSpeed * Time.deltaTime;

        // 限制 x 坐标在 -3 和 3 之间
        newX = Mathf.Clamp(newX, -3f, 3f);

        // 更新位置
        PlayerParent.position = new Vector3(newX, 0, -4);
    }

    #region 发射子弹

    private float fireRate = 1f / 5f; // 每秒 5 发
    private float lastFireTime = 0f; // 记录上次开火时间

    private void Attack()
    {
        //每个对象每秒向前发射 5发子弹

        if (Time.time - lastFireTime < fireRate) return; // 控制开火间隔

        lastFireTime = Time.time; // 更新上次开火时间

        for (int i = 0; i < playerList.Count; i++)
        {
            try
            {
                Shoot(playerList[i].position);
            }
            catch
            {
                RemovePlayer(playerList[i]);
            }
        }
    }

    private void Shoot(Vector3 position)
    {
        // 创建子弹
        GameObject bullet = GameObject.Instantiate(Resources.Load<GameObject>("Bullet"));

        // 添加 Bullet 脚本以使用静态的 bulletData
        bullet.AddComponent<Bullet>().Init(position);
    }

    #endregion

    public void Update()
    {
        Move();

        Attack();
    }

    async void RemovePlayer(Transform player)
    {
        playerList.Remove(player);
        GameObject.Destroy(obj.gameObject);
        Count = playerList.Count;
        if (Count <= 0)
        {
            MessageManager.Broadcast(CMD.EVENT_GAME_LOSE);
        }

        //延迟1秒后重新调整位置
        await Task.Delay(1000);
        AdjustPosition();
    }

   
}