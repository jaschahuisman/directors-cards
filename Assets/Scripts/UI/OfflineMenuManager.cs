using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OfflineMenuManager : MonoBehaviour
{
    [SerializeField] private Button startButton;

    private NetworkManagerExtended network;
    private NetworkManagerExtended Network
    {
        get
        {
            if (network != null) { return network; }
            return network = NetworkManagerExtended.singleton as NetworkManagerExtended;
        }
    }

    private void Start()
    {
        startButton.onClick.AddListener(StartServer);
    }

    private void StartServer()
    {
        Network.StartServer();
    }
}
