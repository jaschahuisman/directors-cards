using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using Mirror;
using Mirror.Discovery;

public class LobbyManager : MonoBehaviour
{
    [Header("Camera objecten")]
    [SerializeField] private GameObject spelersCamera;
    [SerializeField] private GameObject regisseurCamera;

    [Header("Lobby canvas (speler)")]
    [SerializeField] private Button spelerVerbindKnop;

    [Header("Lobby canvas (regisseur)")]
    [SerializeField] private Button regisseurStartServerKnop;

    [Header("Netwerkgegevens")]
    public NetwerkManager netwerk;
    public NetworkDiscovery discovery;

    private void Start()
    {
        // Maak de scene gereed
        SetupEventListeners();
        SetupGebruiker();
    }

    private void SetupEventListeners()
    {
        // Voeg eventlisteners to aan de canvas UI knoppen
        regisseurStartServerKnop.onClick.AddListener(StartServer);
        spelerVerbindKnop.onClick.AddListener(VerbindSpeler);

        // Stel verbindingsfunctie in
        discovery.OnServerFound.AddListener(OnServerFound);
    }

    private void SetupGebruiker()
    {
        // Check of de speler een VR headset op heeft
        // door te controleren of er VR "userpresence" is
        bool isVRSpeler;
        InputDevices
            .GetDeviceAtXRNode(XRNode.Head)
            .TryGetFeatureValue(CommonUsages.userPresence, out isVRSpeler);

        // Activeer de juiste camera voor het juiste devicetype
        spelersCamera.SetActive(isVRSpeler);
        regisseurCamera.SetActive(!isVRSpeler);
    }

    private void StartServer()
    {
        // Start een server op het netwerk
        NetwerkManager.singleton.StartServer();
        discovery.AdvertiseServer();

        // Geef user feedback weer
        spelerVerbindKnop.GetComponentInChildren<Text>().text = "Server starten...";
    }

    private void VerbindSpeler()
    {
        // Geef user feedback weer
        spelerVerbindKnop.GetComponentInChildren<Text>().text = "Server zoeken...";

        // Zoek een verbinding
        discovery.StartDiscovery();
    }

    private void OnServerFound(ServerResponse response)
    {
        // Verbind de speler door naar de server
        netwerk.StartClient(response.uri);
    }
}