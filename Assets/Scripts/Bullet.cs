using UnityEngine;

public class Bullet : MonoBehaviour
{
    // 直接使用静态的 bulletData
    public void Init(Vector3 position)
    {
        transform.position = position + Vector3.forward * 0.5f;
        transform.localScale = Vector3.one * 0.4f;

        transform.name = "Bullet";

        // 设置子弹的初始颜色
        GetComponent<MeshRenderer>().material.color = BulletData.color;

        // 给子弹设置颜色和攻击力，这里通过静态数据自动设置
        gameObject.GetComponent<MeshRenderer>().material.color = BulletData.color; // 使用共享的颜色

        // 给子弹添加 Rigidbody 以使其能移动
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.velocity = Vector3.forward * 10f; // 子弹向前飞行

        transform.localScale = BulletData.zeroScale; // 设置子弹大小
        // 5秒后销毁
        Destroy(gameObject, 5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.name == "Buff")
        {
            Destroy(gameObject);
        }

        if (other.transform.name == "Enemy")
        {
            Destroy(gameObject);
        }

        if (other.transform.name == "Boss")
        {
            Destroy(gameObject);
            MessageManager.Broadcast(CMD.EVENT_BOSS_ATTACK);
        }
    }
}

public static class BulletData
{
    public static int lv = 1; // 所有子弹共享的等级
    public static Color color = Color.yellow; // 所有子弹共享的颜色

    public static int attack = 1; // 所有子弹共享的攻击力

    //设置子弹大小
    public static Vector3 zeroScale = new Vector3(0.1f, 0.1f, 0.5f);

    // 静态方法来升级数据
    public static void LvUp()
    {
        lv++; // 升级等级
        color = Random.ColorHSV(); // 更新颜色
        attack++; // 增加攻击力
        if (zeroScale.x > 0.3f) return; // 防止子弹大小超过1
        zeroScale += new Vector3(0.1f, 0.1f, 0.1f); // 增加子弹大小
    }

    public static void Reset()
    {
        lv = 1;
        color = Color.yellow;
        attack = 1;
        zeroScale = new Vector3(0.1f, 0.1f, 0.5f);
    }
}