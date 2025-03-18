using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Pool;
using System.Collections;

public class CustomConsole : MonoBehaviour
{
    #region Singleton
    public static CustomConsole Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    #endregion

    string output = "";
    string rawOutput = "";
    string stack = "";
    bool warning = true;
    bool normal = true;
    bool error = true;
    [Header("Logging type")]
    [SerializeField] StackTraceLength chosenType;

    [Header("UI Buttons")]
    [SerializeField] GameObject normalLogButton;
    [SerializeField] GameObject warningLogButton;
    [SerializeField] GameObject errorLogButton;

    [Header("UI Components")]
    [SerializeField] GameObject poolHolder;
    [SerializeField] GameObject commandHolder;
    [SerializeField] GameObject commandScrollView;
    [SerializeField] GameObject logArea;

    [Header("Other")]
    [SerializeField] TextMeshProUGUI logPrefab;
    [SerializeField] Animator animator;
    [SerializeField] int amountKeptInHistory;
    [HideInInspector] public bool closed = false;
    [SerializeField] GridLayoutGroup gridLayoutGroup;
    Color normalLogButtonStartColor;
    Color warningLogButtonStartColor;
    Color errorLogButtonStartColor;
    ObjectPool<TextMeshProUGUI> pool;

    float shortSizeY = 30;
    float normalSizeY = 100;
    enum StackTraceLength
    {
        None,
        Normal,
        Short
    }
    private void Start()
    {
        commandScrollView.SetActive(false);
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
        if (Input.GetKeyDown(KeyCode.P))
        {
            UnityEngine.Debug.Log("Normal log");
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            UnityEngine.Debug.LogError("Warning log");
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            UnityEngine.Debug.LogWarning("Warning log");
        }
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
                gridLayoutGroup.cellSize.Set(gridLayoutGroup.cellSize.x, shortSizeY);
                component.text = output;
                break;
            case StackTraceLength.Normal:
                gridLayoutGroup.cellSize.Set(gridLayoutGroup.cellSize.x, normalSizeY);
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
        if (commandScrollView.activeInHierarchy) commandScrollView.SetActive(false);
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
