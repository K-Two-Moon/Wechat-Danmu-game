using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Buff : MonoBehaviour
{
    private TextMeshProUGUI text;
    private RectTransform textRectTransform;
    private Animator textAnimator;
    int count = 0;
    bool isAddCount = false;

    /// <summary>
    /// 确保只能给玩家加一次buff
    /// </summary>
    bool isToPlayer = false;


    public void Init(Vector3 position)
    {
        count = Random.Range(-1, -10);

        gameObject.name = "Buff";
        gameObject.transform.position = position;
        gameObject.GetComponent<MeshRenderer>().material.color = Random.ColorHSV(0, 1, 1, 1, 0.5f, 0.2f); // 随机颜色

        GameObject textPrefab = Resources.Load<GameObject>("Text");
        GameObject buffText = Instantiate(textPrefab);
        buffText.transform.SetParent(GameObject.Find("Canvas").transform, false);
        text = buffText.GetComponent<TextMeshProUGUI>();
        textAnimator = buffText.GetComponent<Animator>();
        textRectTransform = text.transform as RectTransform;
        if (text == null)
        {
            Debug.LogError("buff text is null");
        }

        //显示每秒最多加三次数量，帮我想个方法名
        IsAddCountTrue();
    }

    async private void IsAddCountTrue()
    {
        while (Application.isPlaying && this != null)
        {
            isAddCount = true;
            await Task.Delay(333);
        }
    }

    void Update()
    {
        if (text?.gameObject != null)
        {
            textRectTransform.position = gameObject.transform.position + Vector3.back * 1f + Vector3.up * 0.5f;
            if (count >= 0)
                text.text = "+" + count;
            else
                text.text = count.ToString();
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.name == "Bullet")
        {
            if (isAddCount)
            {
                count++;
                isAddCount = false;
                textAnimator.SetTrigger("Add");
            }
        }

        if (other.transform.name == "Player")
        {
            //Player.Instance.AddPlayer(Player.count + Random.Range(1, 3)); //增加数量
            //增加玩家数量通知
            if (isToPlayer == false)
            {
                Debug.Log(count);
                MessageManager.Broadcast(CMD.EVENT_ADD_PLAYER, count);
                isAddCount = true;
                isToPlayer = true;
            }


            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (text != null)
            Destroy(text?.gameObject);
    }
}