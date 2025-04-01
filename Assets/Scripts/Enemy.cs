using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    private TextMeshProUGUI text;
    private Animator textAnimator;
    int count = 0;
    int state = 0;

    public void Init(Vector3 position)
    {
        gameObject.transform.position = position;
        MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
        mr.material.color = Color.red; // 敌人为红色

        GameObject textPrefab = Resources.Load<GameObject>("Text");
        GameObject buffText = Instantiate(textPrefab);
        buffText.transform.SetParent(GameObject.Find("Canvas").transform, false);
        text = buffText.GetComponent<TextMeshProUGUI>();
        textAnimator = buffText.GetComponent<Animator>();
        if (text == null)
        {
            Debug.LogError("buff text is null");
        }

        count = Random.Range(1, 100);
    }

    void Update()
    {
        if (text?.gameObject != null)
        {
            (text.transform as RectTransform).position =
                gameObject.transform.position + Vector3.up * 2f;
            text.text = count.ToString();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.name == "Bullet")
        {
            Destroy(other.gameObject);
            count -= BulletData.attack;
            textAnimator.SetTrigger("Add");
            if (count <= 0)
            {
                text.gameObject.SetActive(false);
                transform.localScale = Vector3.one;
                state = 1; // 死亡状态
            }
        }

        if (other.transform.name == "Player")
        {
            if (state == 0)
            {
                // Player.Instance.RemovePlayer(other.transform); // 玩家移除.
                // Destroy(other.gameObject); // 玩家死亡.
                //通知死亡的小兵
                MessageManager.Broadcast(CMD.EVENT_DESTORY, other.transform);
            }
            else if (state == 1)
            {
                MessageManager.Broadcast(CMD.EVENT_BULLET_UPGRADE);
                //BulletData.LvUp(Random.ColorHSV());
                Destroy(gameObject);
            }
        }
    }

    private void OnDestroy()
    {
        if (text != null)
            Destroy(text?.gameObject);
    }
}