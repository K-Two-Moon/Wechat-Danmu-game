using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class EnemoyBase
{
    protected GameObject obj;
    public abstract void Init();
    public abstract void Destroy();

    public virtual void Update(float moveSpeedEnemy)
    {
        obj.transform.Translate(Vector3.back * (moveSpeedEnemy * Time.deltaTime), Space.World);
    }
}

public class Boss : EnemoyBase
{
    private TextMeshProUGUI text;
    private RectTransform textRectTransform;
    private Animator textAnimator;
    private AnimatorStateInfo stateInfo;
    private int count = 0;

    public override void Init()
    {
        obj = GameObject.Instantiate(Resources.Load<GameObject>("Boss"));
        obj.name = "Boss";
        obj.transform.position = new Vector3(0, 0, 25);
        obj.transform.rotation = Quaternion.Euler(0, 180, 0);


        GameObject textPrefab = Resources.Load<GameObject>("Text");
        GameObject hpText = GameObject.Instantiate(textPrefab);
        hpText.transform.SetParent(GameObject.Find("Canvas").transform, false);
        text = hpText.GetComponent<TextMeshProUGUI>();
        textAnimator = hpText.GetComponent<Animator>();
        stateInfo = textAnimator.GetCurrentAnimatorStateInfo(1);
        textRectTransform = hpText.GetComponent<RectTransform>();

        count = 1000;
        text.text = count.ToString();

        BindBone();
        AddEvent();
    }

    private void AddEvent()
    {
        MessageManager.AddListener(CMD.EVENT_BOSS_ATTACK, () =>
        {
            if (obj == null) return;
            count -= BulletData.attack;
            text.text = count.ToString();

            if (stateInfo.normalizedTime < 1.0f)
            {
                textAnimator.Play("Add");
            }

            if (count <= 0)
            {
                //发送游戏胜利事件
                MessageManager.Broadcast(CMD.EVENT_GAME_WIN);
                Destroy();
            }
        });
    }

    /// <summary>
    /// 绑定蒙皮骨骼
    /// </summary>
    private void BindBone()
    {
        SkinnedMeshRenderer sk = obj.AddComponent<SkinnedMeshRenderer>();
        SkinnedMeshRenderer[] clothes = new SkinnedMeshRenderer[3];
        for (int i = 0; i < 3; i++)
        {
            clothes[i] = obj.transform.GetChild(i + 1).GetComponent<SkinnedMeshRenderer>();
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

        Transform[] allBone = obj.GetComponentsInChildren<Transform>();
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
    }


    public override void Destroy()
    {
        GameObject.Destroy(text?.gameObject);
        GameObject.Destroy(obj);
        text = null;
        textAnimator = null;
        textRectTransform = null;
        obj = null;
    }

    public override void Update(float moveSpeedEnemy)
    {
        base.Update(moveSpeedEnemy);
        if (textRectTransform != null)
            textRectTransform.position = obj.transform.position + Vector3.up * 2 + Vector3.back * 1;

        if (obj.transform.position.z < 0)
        {
            MessageManager.Broadcast(CMD.EVENT_GAME_LOSE);
        }
    }
}