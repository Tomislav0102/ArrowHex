using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

namespace TempSetup
{
    public class TempGameManager : MonoBehaviour
    {
        bool _isSubscribed;
        void Start()
        {
            DontDestroyOnLoad(this);
            
        }

        void OnSceneEvent(SceneEvent sceneevent)
        {
            SceneEventType tip = sceneevent.SceneEventType;
            switch (tip)
            {
                case SceneEventType.Load:
                    print("Load");
                    break;
                case SceneEventType.Unload:
                    print("Unload");
                    break;
                case SceneEventType.Synchronize:
                    print("Synchronize");
                    break;
                case SceneEventType.ReSynchronize:
                    print("ReSynchronize");
                    break;
                case SceneEventType.LoadEventCompleted:
                    print("LoadEventCompleted");
                    break;
                case SceneEventType.UnloadEventCompleted:
                    print("UnloadEventCompleted");
                    break;
                case SceneEventType.LoadComplete:
                    print("LoadComplete");
                    break;
                case SceneEventType.UnloadComplete:
                    print("UnloadComplete");
                    break;
                case SceneEventType.SynchronizeComplete:
                    print("SynchronizeComplete");
                    break;
                case SceneEventType.ActiveSceneChanged:
                    print("ActiveSceneChanged");
                    break;
                case SceneEventType.ObjectSceneChanged:
                    print("ObjectSceneChanged");
                    break;
            }
        }

        public void StartHost()
        {
            NetworkManager.Singleton.StartHost();
            LoadScene();
        }

        public void StartClient()
        {
            NetworkManager.Singleton.StartClient();
            LoadScene();
        }

        public void LoadNext()
        {
            NetworkManager.Singleton.SceneManager.LoadScene("SomeOtherScene", LoadSceneMode.Single);
        }
        public void LoadScene()
        {
            if (!_isSubscribed)
            {
                _isSubscribed = true;
                NetworkManager.Singleton.SceneManager.OnSceneEvent += OnSceneEvent;
            }
            NetworkManager.Singleton.SceneManager.LoadScene("TestScene", LoadSceneMode.Single);
        }
    }
}
