using System;
using UnityEngine;
[AttributeUsage(AttributeTargets.Method)]
public class CustomCommand : Attribute 
{
    public string ToolTip { get; }
    public CustomCommand(string toolTip)
    {
        this.ToolTip = toolTip;
    }
}