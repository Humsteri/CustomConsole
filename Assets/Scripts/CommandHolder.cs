using System;
using UnityEngine;

public class CommandHolder : MonoBehaviour
{
    [CustomCommand("ADD TOOLTIP TEXT HERE")]
    public void TestCommand()
    {
        print("Hello from script");
    }
    [CustomCommand("XD")]
    public void AnotherCommand()
    {
        print("Hello from script");
    }
    [CustomCommand("For joni")]
    public void Jahoo()
    {
        print("Hello from script");
    }
}
