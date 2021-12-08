using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class LobbyManager : MonoBehaviour
{
    [Header("Camera objecten")]
    [SerializeField] private GameObject spelersCamera;
    [SerializeField] private GameObject regisseurCamera;

    [Header("Lobby canvas (speler)")]
    [SerializeField] private Button spelerVerbindKnop;

    [Header("Lobby canvas (regisseur)")]
    [SerializeField] private Button regisseurStartServerKnop;

    // De static netwerk instance
    private NetwerkManager netwerk;

    private void Start()
    {
        // Sla de static netwerk instance op
        netwerk = NetwerkManager.Instance;

        // Maak de scene gereed
        SetupEventListeners();
        SetupGebruiker();
    }

    private void SetupEventListeners()
    {
        // Voeg eventlisteners to aan de canvas UI knoppen
        regisseurStartServerKnop.onClick.AddListener(StartServer);
        spelerVerbindKnop.onClick.AddListener(VerbindSpeler);
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
        netwerk.StartServer();

        // Geef user feedback weer
        spelerVerbindKnop.GetComponentInChildren<Text>().text = "Server starten...";
    }

    private void VerbindSpeler()
    {
        // Stel verbindingsfunctie in
        netwerk.discovery.OnServerFound.AddListener((verbinding) => netwerk.StartClient(verbinding.uri));

        // Geef user feedback weer
        spelerVerbindKnop.GetComponentInChildren<Text>().text = "Server zoeken...";

        // Zoek een verbinding
        netwerk.discovery.StartDiscovery();
    }
}