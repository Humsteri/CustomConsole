using NUnit.Framework;
using System;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class ExecutingCustomCommand : MonoBehaviour
{

    [SerializeField] TMP_InputField inputField;
    List<MethodInfo> customMethodsList = new();
    List<Action> actions = new();
    public Dictionary<MethodInfo, Component> keyValuePairs = new Dictionary<MethodInfo, Component>();
    public void ExecuteCommand()
    {
        ExecuteCommand(inputField.text);
        
    }
    private void Start()
    {
        GetMethods();
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
                    // Check if the method has the CustomCommand attribute
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
                    Debug.Log($"Executed Command: {entry.Key.Name}");
                    inputField.text = "";
                    return;
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error invoking method: {ex.Message}");
                    print(entry.Value.gameObject.GetComponent<MonoBehaviour>());
                    inputField.text = "";
                }
            }
        }
        Debug.LogError("No command found with: " + commandName);
        inputField.text = "";
    }

}
