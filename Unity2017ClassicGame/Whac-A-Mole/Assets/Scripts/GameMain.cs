using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameMain : MonoBehaviour
{
    private TimeManager _timeManager;
    private Button startBtn;
    private Text timeText;
    private Text scoreText;
    private List<Transform> holeList = new List<Transform>();
    
    // 游戏时间
    private int gameTime = 60;
    private void Start()
    {
        _timeManager = gameObject.AddComponent<TimeManager>();
        // 按钮注册
        startBtn = transform.Find("Panel/Button").GetComponent<Button>();
        timeText = transform.Find("Panel/timeText").GetComponent<Text>();
        scoreText = transform.Find("Panel/scoreText").GetComponent<Text>();
        ScoreManager.AddListener(RefreshScore);
        startBtn.onClick.AddListener(StartGameClick);

        Transform holeParent = transform.Find("Panel/Hole");
        for (int i = 0; i < holeParent.childCount; i++)
        {
            holeList.Add(holeParent.GetChild(i));
        }
    }

    private void RefreshScore(int arg0)
    {
        scoreText.text = string.Format("游戏得分: {0}", arg0);
    }

    private void StartGameClick()
    {
        // 开始计时
        _timeManager.AddTime(gameTime,TimeCallBack);
        ScoreManager.Restart();
        InvokeRepeating("CreateGopher", 0, 3);
        startBtn.gameObject.SetActive(false);
    }

    private void TimeCallBack(float tick)
    {
        int time = (int) tick;
        timeText.text = string.Format("剩余时间: {0}", time);
        if (tick < 0)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        _timeManager.StopTime();
        CancelInvoke("CreateGopher");
        startBtn.gameObject.SetActive(true);
        Debug.Log("游戏结束");
    }

    private void CreateGopher()
    {
        int i = Random.Range(0, holeList.Count);
        GameObject gopher = ResManager.LoadPrefab("Gophers");
        GameObject gopherObj = Instantiate(gopher, holeList[i]);
        gopherObj.AddComponent<Gophers>();
    }
}