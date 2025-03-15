using System;
using System.Diagnostics;
using System.Linq;
using TMPro;
using UnityEditor.Searcher;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class CustomConsole : MonoBehaviour
{
    public string output = "";
    public string stack = "";
    [SerializeField] GameObject logArea;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            print("Ya hoo");
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            UnityEngine.Debug.LogError("Ya 123123");
        }
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        output = "["+ System.DateTime.Now.ToString("H:mm:ss") + "]" + " " + logString;
        stack = stackTrace;
        GameObject uiLog = new GameObject("Log");
        TextMeshProUGUI component = uiLog.AddComponent<TextMeshProUGUI>();
        component.text = output + ", From: " + stackTrace.Split(new string[] { "(at " }, StringSplitOptions.None).Last();
        component.fontSize = 30;
        uiLog.transform.SetParent(logArea.transform, false);
        switch (type)
        {
            case LogType.Error:
                component.color = Color.red;
                break;
            case LogType.Assert:
                break;
            case LogType.Warning:
                component.color = Color.yellow;
                break;
            case LogType.Log:
                component.color = Color.white;
                break;
            case LogType.Exception:
                break;
            default:
                break;
        }
    }
    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }
    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }
}
