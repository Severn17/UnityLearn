using System;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class GameMain : MonoBehaviour
{
    private TimeManager _timeManager;
    private Button startBtn;
    private void Start()
    {
        ScoreManager.Init();
        _timeManager = gameObject.AddComponent<TimeManager>();
        // 按钮注册
        startBtn = transform.Find("Panel/Button").GetComponent<Button>();
        startBtn.onClick.AddListener(StartGameClick);
    }

    private void StartGameClick()
    {
        // 开始计时
        _timeManager.AddTime(60,TimeCallBack);
    }

    private void TimeCallBack(float tick)
    {
        Debug.Log("剩余时间: " + tick);
    }
}