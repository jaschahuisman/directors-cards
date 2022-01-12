using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.Discovery;
using UnityEngine.SceneManagement;

public class NetworkManagerExtended : NetworkManager
{
    [Header("Gameplay scene")]
    [Scene] [SerializeField] private string gameplayScene = string.Empty;

    [Header("Playerlists")]
    public List<NetworkPlayer> networkPlayers = new List<NetworkPlayer>();
    public List<NetworkPlayer> briefedPlayers = new List<NetworkPlayer>();
    public List<NetworkPlayer> playingPlayers = new List<NetworkPlayer>();

    public override void OnStartServer()
    {
        GetComponent<NetworkDiscovery>().AdvertiseServer();
        base.OnStartServer();
    }

    public override void OnStopServer()
    {
        networkPlayers.Clear();
        briefedPlayers.Clear();
        playingPlayers.Clear();

        base.OnStopServer();
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        GameObject player = Instantiate(playerPrefab);
        NetworkServer.AddPlayerForConnection(conn, player);
    }

    public void LoopTroughPlayers(Action<NetworkPlayer> function, List<NetworkPlayer> playerList = null)
    {
        if (playerList == null)
            playerList = networkPlayers;
        
        foreach (NetworkPlayer player in playerList)
            function(player);
    }

    public void ExecuteIfLocalPlayer(Action function)
    {
        LoopTroughPlayers(
           (NetworkPlayer player) => { if (player.isLocalPlayer) function(); }
        );
    }
}
