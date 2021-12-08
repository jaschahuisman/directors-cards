using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.Discovery;

public class NetwerkManager : NetworkManager
{
    // Alle client-verbindingen
    [HideInInspector] public List<NetworkConnection> verbindingen = new List<NetworkConnection>();
    // De netwerk "zoeker"
    [HideInInspector] public NetworkDiscovery discovery;

    // De static netwerk instance
    public static NetwerkManager Instance;
    

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
        base.OnServerConnect(verbinding);
    }

    public override void OnServerDisconnect(NetworkConnection verbinding)
    {
        // Verwijder verbroken verbinding uit client-verbindingen
        verbindingen.Remove(verbinding);
        base.OnServerConnect(verbinding);
    }
}
