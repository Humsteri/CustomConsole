using UnityEngine;

public class CommandHolder : MonoBehaviour
{
    [CustomCommand]
    public void Hello()
    {
        print("Helloo");
    }
    
}
