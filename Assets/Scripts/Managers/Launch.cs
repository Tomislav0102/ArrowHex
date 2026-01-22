using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

public class Launch : MonoBehaviour
{
    public static Launch Instance;
    public MyLobbyManager myLobbyManager;
    public PersistentDataManager persistentDataManager;
    public MyAuthManager myAuthManager;
    public LocalDataManager localDataManager;
    public ApiClientManager apiClientManager;

    [Title("Debug display")]
    public bool showDebugBackend;
    
    
    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);
    }
}

