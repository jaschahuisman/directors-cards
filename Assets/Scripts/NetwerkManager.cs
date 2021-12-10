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
    public static Action<int> OnVerbindingChange;

    // De static netwerk instance
    public static NetwerkManager Instance;
    
    public override void Awake()
    {
        // Maak de static netwerk instance
        Instance = this;
        base.Awake();
    }

    public override void OnServerConnect(NetworkConnection verbinding)
    {
        // Voeg nieuwe verbinding toe aan client-verbindingen
        verbindingen.Add(verbinding);
        base.OnServerConnect(verbinding);
    }

    public override void OnServerDisconnect(NetworkConnection verbinding)
    {
        // Verwijder verbroken verbinding uit client-verbindingen
        verbindingen.Remove(verbinding);
        OnVerbindingChange?.Invoke(verbindingen.Count);
        base.OnServerDisconnect(verbinding);
    }

    public override void OnServerAddPlayer(NetworkConnection verbinding)
    {
        // Maak een speler instance van de playerPrefab en
        // krijg toegang tot het "NetwerkSpeler" component
        GameObject speler = Instantiate(playerPrefab);
        NetwerkSpeler netwerkSpeler = speler.GetComponent<NetwerkSpeler>();


        // Voeg de speler toe aan de netwerk server
        NetworkServer.AddPlayerForConnection(verbinding, speler);

        OnVerbindingChange?.Invoke(verbindingen.Count);

        // Na het toevoegen van de speler aan de server:
        // update de identiteit van de speler
        SpelerId spelerId = (verbindingen.Count % 2 == 1) ? SpelerId.Speler1 : SpelerId.Speler2;
        netwerkSpeler.spelerId = spelerId;                      // Update server variabele
        netwerkSpeler.UpdateSpelerIdentiteit(spelerId);         // Update client variabele
    }
}
