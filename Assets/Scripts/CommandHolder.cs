using UnityEngine;

public class CommandHolder : MonoBehaviour
{
    [CustomCommand]
    public void TestCommand()
    {
        print("Hello from script");
    }
}
