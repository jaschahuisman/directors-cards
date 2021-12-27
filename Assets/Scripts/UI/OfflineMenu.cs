using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror.Discovery;

public class OfflineMenu : MonoBehaviour
{
    [SerializeField] private Button startServerButton;
    [SerializeField] private Button connectButtonVr;
    [SerializeField] private Button connectButtonTablet;

    private NetworkManagerExt network;
    private NetworkManagerExt Network
    {
        get
        {
            if (network != null) { return network; }
            return network = NetworkManagerExt.singleton as NetworkManagerExt;
        }
    }

    private void Start()
    {
        startServerButton.onClick.AddListener(() => Network.StartServer());
        connectButtonVr.onClick.AddListener(() => JoinServer(PlayerDevice.VR));
        connectButtonTablet.onClick.AddListener(() => JoinServer(PlayerDevice.Tablet));

        Network.GetComponent<NetworkDiscovery>().OnServerFound.AddListener(ConnectPlayer);
    }

    private void JoinServer(PlayerDevice device)
    {
        PlayerPrefs.SetString("device", device.ToString());
        Network.GetComponent<NetworkDiscovery>().StartDiscovery();
    }

    private void ConnectPlayer(ServerResponse response)
    {
        Network.StartClient(response.uri);
    }
}
