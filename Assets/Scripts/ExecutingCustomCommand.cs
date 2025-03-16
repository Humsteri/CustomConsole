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
    Dictionary<MethodInfo, GameObject> keyValuePairs = new Dictionary<MethodInfo, GameObject>();
    public void ExecuteCommand()
    {
        ExecuteCommand(inputField.text);
        inputField.text = "";
    }
    private void Start()
    {
        GetMethods();
    }
    void GetMethods()
    {
        foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>())
        {
            if (go.transform.childCount > 0)
            {
                MethodsRecursively(go);
            }
            foreach (Component component in go.GetComponents<Component>())
            {
                Type type = component.GetType();

                // Loop through all methods in this component's type
                foreach (MethodInfo method in type.GetMethods())
                {
                    // Check if the method has the CustomCommand attribute
                    if (method.GetCustomAttribute<CustomCommand>() != null)
                    {
                        keyValuePairs.Add(method, go);
                    }
                }
            }
        }
    }
    void MethodsRecursively(GameObject obj)
    {
        for (int i = 0; i < obj.transform.childCount; i++)
        {
            if (obj.transform.GetChild(i).childCount > 0)
            {
                MethodsRecursively(obj.transform.GetChild(i).gameObject);
            }
            foreach (Component component in obj.transform.GetChild(i).GetComponents<Component>())
            {
                Type type = component.GetType();

                // Loop through all methods in this component's type
                foreach (MethodInfo method in type.GetMethods())
                {
                    // Check if the method has the CustomCommand attribute
                    if (method.GetCustomAttribute<CustomCommand>() != null)
                    {
                        keyValuePairs.Add(method, obj.transform.GetChild(i).gameObject);
                        //print("Found:" + obj.transform.GetChild(i).gameObject + " " + method);
                    }
                }
            }
        }
    }
    public void ExecuteCommand(string commandName)
    {
        foreach (KeyValuePair<MethodInfo, GameObject> entry in keyValuePairs)
        {
            if (entry.Key.Name.Equals(commandName, StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    entry.Key.Invoke(entry.Value.gameObject.GetComponent<MonoBehaviour>(), null);
                    Debug.Log($"Executed Command: {entry.Key.Name}");
                    return;
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error invoking method: {ex.Message}");
                    print(entry.Value);
                }
            }
        }
    }
}
