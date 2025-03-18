using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Pool;
using System.Collections;

public class CustomConsole : MonoBehaviour
{
    string output = "";
    string rawOutput = "";
    string stack = "";
    bool warning = true;
    bool normal = true;
    bool error = true;
    [SerializeField] StackTraceLength chosenType;
    [SerializeField] GameObject normalLogButton;
    [SerializeField] GameObject warningLogButton;
    [SerializeField] GameObject errorLogButton;
    [SerializeField] GameObject poolHolder;
    [SerializeField] GameObject commandHolder;
    [SerializeField] TextMeshProUGUI logPrefab;
    [SerializeField] int amountKeptInHistory;
    [SerializeField] ObjectPool<TextMeshProUGUI> pool;
    Color normalLogButtonStartColor;
    Color warningLogButtonStartColor;
    Color errorLogButtonStartColor;
    [SerializeField] ExecutingCustomCommand command;
    [SerializeField] Animator animator;
    bool closed = false;
    enum StackTraceLength
    {
        None,
        Normal,
        Short
    }
    [SerializeField] GameObject logArea;
    private void Start()
    {
        pool = new ObjectPool<TextMeshProUGUI>(() =>
        {
            return Instantiate(logPrefab, poolHolder.transform);
        }, logPrefab =>
        {
            logPrefab.gameObject.SetActive(true);
        }, logPrefab =>
        {
            logPrefab.gameObject.SetActive(false);
            logPrefab.transform.SetParent(poolHolder.transform);
        }, logPrefab =>
        {
            Destroy(logPrefab);
        },false,5, 20);
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
            UnityEngine.Debug.LogError("Warning log");
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            UnityEngine.Debug.LogWarning("Warning log");
        }
    }
    [CustomCommand("")]
    public void He()
    {
        print("Jippii");
    }
    void HandleLog(string logString, string stackTrace, LogType type)
    {
        Color chosenColor = new Color();
        switch (type)
        {
            case LogType.Error:
                if (!error) return;
                chosenColor = Color.red;
                break;
            case LogType.Assert:
                break;
            case LogType.Warning:
                if (!warning) return;
                chosenColor = Color.yellow;
                break;
            case LogType.Log:
                if (!normal) return;
                chosenColor = Color.white;
                break;
            case LogType.Exception:
                break;
            default:
                break;
        }
        rawOutput = logString;
        output = "["+ System.DateTime.Now.ToString("H:mm:ss") + "]" + " " + logString;
        stack = stackTrace;
        TextMeshProUGUI component = pool.Get();
        component.color = chosenColor;
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
        component.transform.SetParent(logArea.transform, false);
        //StartCoroutine(ReleaseAfterTime(component, 5f));
    }
    public void PlayAnimation()
    {
        closed = !closed;
        if (commandHolder.activeInHierarchy) commandHolder.SetActive(false);
        animator.SetBool("Play", closed);
    }
    IEnumerator ReleaseAfterTime(TextMeshProUGUI logItem, float delay)
    {
        yield return new WaitForSeconds(delay);
        pool.Release(logItem);
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
