using NUnit.Framework;
using System;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class ExecutingCustomCommand : MonoBehaviour
{
    #region Singleton
    public static ExecutingCustomCommand Instance { get; private set; }
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

    [Header("UI Components")]
    [SerializeField] TMP_InputField inputField;
    [SerializeField] TextMeshProUGUI suggestText;
    [SerializeField] TextMeshProUGUI suggestPrefab;
    [SerializeField] GameObject commandScrollView;
    [SerializeField] Button commandButton;
    [SerializeField] GameObject suggestArea;
    
    string suggestCommand = "";
    public Dictionary<MethodInfo, Component> keyValuePairs = new Dictionary<MethodInfo, Component>();
    public void ExecuteCommand()
    {
        ExecuteCommand(inputField.text);
        
    }
    private void Start()
    {
        GetMethods();
        ListOfCommands();
    }
    private void Update()
    {
        if (suggestText.text != "" && Input.GetKeyDown(KeyCode.Tab))
        {
            inputField.text = suggestCommand;
            inputField.MoveTextEnd(false);
            suggestText.text = "";
        }
    }
    void GetMethods()
    {
        foreach (GameObject item in GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
        {
            foreach (Component component in item.GetComponents<MonoBehaviour>())
            {
                Type type = component.GetType();
                foreach (MethodInfo method in type.GetMethods())
                {
                    if (method.GetCustomAttribute<CustomCommand>() != null)
                    {
                        if (!keyValuePairs.ContainsKey(method))
                            keyValuePairs.Add(method, component);
                    }
                }
            }
        }
    }
    public void ExecuteCommand(string commandName)
    {
        foreach (KeyValuePair<MethodInfo, Component> entry in keyValuePairs)
        {
            if (entry.Key.Name.Equals(commandName, StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    entry.Key.Invoke(entry.Value, null);
                    UnityEngine.Debug.Log($"Executed Command: {entry.Key.Name}");
                    inputField.text = "";
                    return;
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogError($"Error invoking method: {ex.Message}");
                    print(entry.Value.gameObject.GetComponent<MonoBehaviour>());
                    inputField.text = "";
                }
            }
        }
        UnityEngine.Debug.LogError("No command found with: " + commandName);
        inputField.text = "";  
    }
    public void ExecuteCommandFromButton()
    {
        string commandName = inputField.text;
        foreach (KeyValuePair<MethodInfo, Component> entry in keyValuePairs)
        {
            if (entry.Key.Name.Equals(commandName, StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    entry.Key.Invoke(entry.Value, null);
                    UnityEngine.Debug.Log($"Executed Command: {entry.Key.Name}");
                    inputField.text = "";
                    return;
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogError($"Error invoking method: {ex.Message}");
                    print(entry.Value.gameObject.GetComponent<MonoBehaviour>());
                    inputField.text = "";
                }
            }
        }
        UnityEngine.Debug.LogError("No command found with: " + commandName);
        inputField.text = "";
    }
    public void ListOfCommands()
    {
        if (suggestArea.transform.childCount > 0)
        {
            for (int i = 0; i < suggestArea.transform.childCount; i++)
            {
                Destroy(suggestArea.transform.GetChild(i).gameObject);
            }
        }
        suggestArea.SetActive(true);
        foreach (KeyValuePair<MethodInfo, Component> entry in keyValuePairs)
        {
            Button butt = Instantiate(commandButton, suggestArea.transform);
            butt.GetComponentInChildren<TextMeshProUGUI>().text = entry.Key.Name;
            butt.onClick.AddListener(() => WriteToConsole(entry.Key.Name));
            var attribute = entry.Key.GetCustomAttribute<CustomCommand>();
            butt.GetComponent<PointerBehavior>().hoverText = attribute.ToolTip;
        }
        if (CustomConsole.Instance.closed)
        {
            suggestArea.SetActive(true);
            commandScrollView.SetActive(true);
            UnityEngine.Debug.Log("Updated command list!");
        }
        else
        {
            suggestArea.SetActive(false);
            commandScrollView.SetActive(false);
        }
       
    }
    public void ShowCommands()
    {
        if (!CustomConsole.Instance.closed) return;
        suggestArea.SetActive(suggestArea.activeInHierarchy ? false : true);
        commandScrollView.SetActive(commandScrollView.activeInHierarchy ? false : true);
    }
    public void WriteToConsole(string text)
    {
        inputField.text = text.ToLower();
        inputField.MoveTextEnd(false);
        suggestText.text = "";

    }
    public void SuggestCommand()
    {
        string catchText = inputField.text;
        string currentText = inputField.text;
        if (currentText == "")
        {
            suggestText.text = "";
            suggestCommand = "";
            return;
        }
        foreach (KeyValuePair<MethodInfo, Component> entry in keyValuePairs)
        {
            if (entry.Key.Name.Contains(currentText, StringComparison.OrdinalIgnoreCase))
            {
                string suggestedCompletion = new String(' ', currentText.Length + 10) + "TAB => " + entry.Key.Name.ToLower();
                suggestText.text =  currentText + suggestedCompletion.ToLower();

                suggestCommand = entry.Key.Name.ToLower();
                inputField.text = catchText.ToLower();
                return;
            }
        }
        suggestText.text = "";
        suggestCommand = "";
        inputField.text = catchText.ToLower();
    }
}
