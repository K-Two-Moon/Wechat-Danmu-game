using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMD : MonoBehaviour
{
    /// <summary>
    /// 销毁小兵事件
    /// </summary>
    public const string EVENT_DESTORY = "DESTORY_ENEMY";

    /// <summary>
    /// 子弹升级事件
    /// </summary>
    public const string EVENT_BULLET_UPGRADE = "BULLET_UPGRADE";

    /// <summary>
    /// 增加玩家数量事件
    /// </summary>
    public const string EVENT_ADD_PLAYER = "ADD_PLAYER";

    /// <summary>
    /// Boss被攻击事件
    /// </summary>
    public const string EVENT_BOSS_ATTACK = "BOSS_ATTACK";

    /// <summary>
    /// 游戏胜利事件
    /// </summary>
    public const string EVENT_GAME_WIN = "GAME_WIN";

    /// <summary>
    /// 游戏失败事件
    /// </summary>
    public const string EVENT_GAME_LOSE = "GAME_LOSE";


    /// <summary>
    /// 重新开始游戏事件
    /// </summary>
    public const string EVENT_RESTART_GAME = "RESTART_GAME";
}