using NUnit.Framework;
using System;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using static UnityEngine.EventSystems.EventTrigger;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
public class ExecutingCustomCommand : MonoBehaviour
{

    [SerializeField] TMP_InputField inputField;
    [SerializeField] TextMeshProUGUI suggestText;
    [SerializeField] TextMeshProUGUI suggestPrefab;
    [SerializeField] Button commandButton;
    string suggestCommand = "";
    [SerializeField] GameObject suggestArea;
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
    public void ListOfCommands()
    {
        suggestArea.SetActive(true);
        foreach (KeyValuePair<MethodInfo, Component> entry in keyValuePairs)
        {
            Button butt = Instantiate(commandButton, suggestArea.transform);
            butt.GetComponentInChildren<TextMeshProUGUI>().text = entry.Key.Name;
            butt.onClick.AddListener(() => WriteToConsole(entry.Key.Name));
            var attribute = entry.Key.GetCustomAttribute<CustomCommand>();
            butt.GetComponent<PointerBehavior>().hoverText = attribute.ToolTip;
            //butt.GetComponent<PointerBehavior>().OnPointerEnter()
        }
        suggestArea.SetActive(false);
    }
    public void ShowCommands()
    {
        suggestArea.SetActive(suggestArea.activeInHierarchy ? false : true);
    }
    public void OnMouseOverCommand()
    {

    }
    public void OnMouseExitCommand()
    {

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
