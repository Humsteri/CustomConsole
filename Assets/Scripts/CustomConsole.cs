using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomConsole : MonoBehaviour
{
    string output = "";
    string stack = "";
    bool warning = true;
    bool normal = true;
    bool error = true;
    [SerializeField] StackTraceLength chosenType;
    [SerializeField] GameObject normalLogButton;
    [SerializeField] GameObject warningLogButton;
    [SerializeField] GameObject errorLogButton;
    Color normalLogButtonStartColor;
    Color warningLogButtonStartColor;
    Color errorLogButtonStartColor;
    enum StackTraceLength
    {
        None,
        Normal,
        Short
    }
    [SerializeField] GameObject logArea;
    private void Start()
    {
        normalLogButtonStartColor = normalLogButton.GetComponent<Image>().color;
        warningLogButtonStartColor = warningLogButton.GetComponent<Image>().color;
        errorLogButtonStartColor = errorLogButton.GetComponent<Image>().color;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            print("Normal Log");
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            UnityEngine.Debug.LogError("Error log");
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            UnityEngine.Debug.LogWarning("Warning log");
        }
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {

        output = "["+ System.DateTime.Now.ToString("H:mm:ss") + "]" + " " + logString;
        stack = stackTrace;
        GameObject uiLog = new GameObject("Log");
        TextMeshProUGUI component = uiLog.AddComponent<TextMeshProUGUI>();
        switch (chosenType)
        {
            case StackTraceLength.None:
                component.text = output;
                break;
            case StackTraceLength.Normal:
                component.text = output + ", " + stackTrace;
                break;
            case StackTraceLength.Short:
                component.text = output + ", From: " + stackTrace.Split(new string[] { "(at " }, StringSplitOptions.None).Last();
                break;
            default:
                break;
        }
        component.fontSize = 30;
        uiLog.transform.SetParent(logArea.transform, false);
        switch (type)
        {
            case LogType.Error:
                if (!error)
                {
                    Destroy(uiLog);
                    return;
                }
                component.color = Color.red;
                break;
            case LogType.Assert:
                break;
            case LogType.Warning:
                if (!warning)
                {
                    Destroy(uiLog);
                    return;
                }
                component.color = Color.yellow;
                break;
            case LogType.Log:
                if (!normal)
                {
                    Destroy(uiLog);
                    return;
                }
                component.color = Color.white;
                break;
            case LogType.Exception:
                break;
            default:
                break;
        }
    }
    public void NormalLog()
    {
        normal = normal? false : true;
        normalLogButton.GetComponent<Image>().color = normal ? normalLogButtonStartColor : normalLogButtonStartColor - new Color(0, 0, 0, 0.8f);
    }
    public void ErrorLog()
    {
        error = error ? false : true;
        errorLogButton.GetComponent<Image>().color = error ? errorLogButtonStartColor : errorLogButtonStartColor - new Color(0,0,0,0.8f);
    }
    public void WarningLog()
    {
        warning = warning ? false : true;
        warningLogButton.GetComponent<Image>().color = warning ? warningLogButtonStartColor : warningLogButtonStartColor - new Color(0, 0, 0, 0.8f);
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
