using Unity.Netcode;

public class PlayerStats : NetworkBehaviour
{
    public NetworkVariable<int> KillCount = new(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );
}