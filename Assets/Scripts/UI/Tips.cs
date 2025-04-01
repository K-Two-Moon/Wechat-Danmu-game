using System;
using UnityEngine;
using UnityEngine.UI;

public class Tips : MonoBehaviour
{
    [SerializeField] Text tipsText;

    /// <summary>
    /// 重新开始按钮
    /// </summary>
    [SerializeField] Button restartButton;

    void Start()
    {
        restartButton.onClick.AddListener(GameRestart);

        MessageManager.AddListener(CMD.EVENT_GAME_WIN, GameWin);

        MessageManager.AddListener(CMD.EVENT_GAME_LOSE, GameLose);

        gameObject.SetActive(false);
    }

    private void GameRestart()
    {
        MessageManager.Broadcast(CMD.EVENT_RESTART_GAME);
        gameObject.SetActive(false);
    }

    private void GameWin()
    {
        gameObject.SetActive(true);
        tipsText.text = "恭喜你，你赢了！";
    }

    private void GameLose()
    {
        gameObject.SetActive(true);
        tipsText.text = "很遗憾，你输了！";
    }


    private void OnDestroy()
    {
        restartButton.onClick.RemoveListener(GameRestart);
        MessageManager.RemoveListener(CMD.EVENT_GAME_WIN, GameWin);
        MessageManager.RemoveListener(CMD.EVENT_GAME_LOSE, GameLose);
    }
}