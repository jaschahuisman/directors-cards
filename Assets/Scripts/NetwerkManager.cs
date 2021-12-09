using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.Discovery;

public class NetwerkManager : NetworkManager
{
    // Alle client-verbindingen
    [HideInInspector] public List<NetworkConnection> verbindingen = new List<NetworkConnection>();
    [HideInInspector] public List<NetwerkSpeler> spelers = new List<NetwerkSpeler>();

    // De netwerk "zoeker"
    [HideInInspector] public NetworkDiscovery discovery;

    // De static netwerk instance
    public static NetwerkManager Instance;

    // Event hook die luistert naar wijzigingen in verbindingen
    public static event Action<int> OnVerbindingenChange;
    
    public override void Awake()
    {
        // Maak de static netwerk instance
        Instance = this;
        base.Awake();
    }

    public override void Start()
    {
        // Sla de netwerk "zoeker" op
        discovery = GetComponent<NetworkDiscovery>();
        base.Start();
    }

    public override void OnServerConnect(NetworkConnection verbinding)
    {
        // Voeg nieuwe verbinding toe aan client-verbindingen
        verbindingen.Add(verbinding);
        OnVerbindingenChange?.Invoke(verbindingen.Count);
        base.OnServerConnect(verbinding);
    }

    public override void OnServerDisconnect(NetworkConnection verbinding)
    {
        // Verwijder verbroken verbinding uit client-verbindingen
        verbindingen.Remove(verbinding);
        spelers.Remove(verbinding.identity.GetComponent<NetwerkSpeler>());
        OnVerbindingenChange?.Invoke(verbindingen.Count);
        base.OnServerConnect(verbinding);
    }

    public override void OnServerAddPlayer(NetworkConnection verbinding)
    {
        // Maak een speler instance van de playerPrefab en
        // krijg toegang tot het "NetwerkSpeler" component
        GameObject speler = Instantiate(playerPrefab);
        NetwerkSpeler netwerkSpeler = speler.GetComponent<NetwerkSpeler>();

        // Voeg de speler toe aan de netwerk server
        NetworkServer.AddPlayerForConnection(verbinding, speler);

        // Voeg nieuwe verbinding toe aan client-verbindingen (netwerkspeler)
        spelers.Add(verbinding.identity.GetComponent<NetwerkSpeler>());

        // Na het toevoegen van de speler aan de server:
        // update de identiteit van de speler
        SpelerId spelerId = (verbindingen.Count % 2 == 1) ? SpelerId.Speler1 : SpelerId.Speler2;
        netwerkSpeler.spelerId = spelerId;                      // Update server variabele
        netwerkSpeler.UpdateSpelerIdentiteit(spelerId);         // Update client variabele

        // Debug spelernaam in editor van de server
        netwerkSpeler.gameObject.name = $"Netwerk Speler: ${spelerId}";
    }
}
