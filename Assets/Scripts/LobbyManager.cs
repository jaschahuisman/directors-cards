using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR; 
using Mirror.Discovery;

public class LobbyManager : MonoBehaviour
{
    [Header("Camera objects")]
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private GameObject directorCamera;

    [Header("Lobby canvas (player)")]
    [SerializeField] private Button connectPlayerButton;

    [Header("Lobby canvas (director)")]
    [SerializeField] private Button connectDirectorButton;

    [Header("Netwerkgegevens")]
    public NetworkManagerExtended network;
    public NetworkDiscovery discovery;

    private void Start()
    {
        SetupEventListeners();
        SetupUserByDevice();
    }

    private void SetupEventListeners()
    {
        connectDirectorButton.onClick.AddListener(StartServer);
        connectPlayerButton.onClick.AddListener(ConnectPlayer);

        discovery.OnServerFound.AddListener(OnServerFound);
    }

    private void SetupUserByDevice()
    {
        bool isVrPlayer;
        InputDevices
            .GetDeviceAtXRNode(XRNode.Head)
            .TryGetFeatureValue(CommonUsages.userPresence, out isVrPlayer);

        playerCamera.SetActive(isVrPlayer);
        directorCamera.SetActive(!isVrPlayer);
    }

    private void StartServer()
    {
        NetworkManagerExtended.singleton.StartServer();
        discovery.AdvertiseServer();

        connectPlayerButton.GetComponentInChildren<Text>().text = "Server starten...";
    }

    private void ConnectPlayer()
    {
        discovery.StartDiscovery();

        connectPlayerButton.GetComponentInChildren<Text>().text = "Server zoeken...";
    }

    private void OnServerFound(ServerResponse response)
    {
        network.StartClient(response.uri);
    }
}