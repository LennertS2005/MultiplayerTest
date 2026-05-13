using UnityEngine;

public class NameManager : MonoBehaviour
{
    private string playerName;
    public void SetName(string name)
    {
        playerName = name;
    }
    public string GetName()
    {
            return playerName;
    }
}
