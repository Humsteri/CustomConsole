using UnityEngine;

public class CommandHolder : MonoBehaviour
{
    [CustomCommand]
    public void TestCommand()
    {
        print("Hello from script");
    }
    [CustomCommand]
    public void AnotherCommand()
    {
        print("Hello from script");
    }
    [CustomCommand]
    public void Jahoo()
    {
        print("Hello from script");
    }
}
