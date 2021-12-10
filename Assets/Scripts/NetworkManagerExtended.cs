using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.Discovery;

public class NetworkManagerExtended : NetworkManager
{
    public static NetworkManagerExtended Instance;

    [HideInInspector] public List<NetworkConnection> connections = new List<NetworkConnection>();
    public static Action<int> OnConnectionEvent;
    
    public override void Awake()
    {
        Instance = this;
        base.Awake();
    }

    public override void OnServerConnect(NetworkConnection verbinding)
    {
        connections.Add(verbinding);
        base.OnServerConnect(verbinding);
    }

    public override void OnServerDisconnect(NetworkConnection verbinding)
    {
        connections.Remove(verbinding);
        OnConnectionEvent?.Invoke(connections.Count);
        base.OnServerDisconnect(verbinding);
    }

    public override void OnServerAddPlayer(NetworkConnection verbinding)
    {
        GameObject speler = Instantiate(playerPrefab);
        NetworkPlayer netwerkSpeler = speler.GetComponent<NetworkPlayer>();

        NetworkServer.AddPlayerForConnection(verbinding, speler);

        OnConnectionEvent?.Invoke(connections.Count);

        PlayerId spelerId = (connections.Count % 2 == 1) ? PlayerId.Player1 : PlayerId.Player2;
        
        netwerkSpeler.UpdateSpelerIdentiteit(spelerId);         
        netwerkSpeler.id = spelerId;                      
    }
}
