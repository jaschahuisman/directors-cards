using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.Discovery;


public class NetworkManagerExt : NetworkManager
{
    [Header("Lobby")]
    [Scene] [SerializeField] private string lobbyScene = string.Empty;
    [SerializeField] private GameObject spectatorPrefab;

    [Header("Gameplay")]
    [Scene] [SerializeField] private string gameplayScene = string.Empty;

    public List<NetworkPlayer> NetworkPlayers = new List<NetworkPlayer>();
    public List<NetworkSpectator> NetworkSpectators = new List<NetworkSpectator>();

    public static event Action<bool> ReadyStateEvent;
    public static event Action ConnectionEvent;

    #region Server
    public override void OnStartServer()
    {
        GetComponent<NetworkDiscovery>().AdvertiseServer();
        base.OnStartServer();
    }

    public override void OnStopServer()
    {
        NetworkPlayers.Clear();
        NetworkSpectators.Clear();

        base.OnStopServer();
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        if (conn.identity != null && conn.identity.GetComponent<NetworkPlayer>())
        {
            var player = conn.identity.GetComponent<NetworkPlayer>();
            NetworkPlayers.Remove(player);
            NotifyReadyState();
            Destroy(player.gameObject);
        }

        if (conn.identity != null && conn.identity.GetComponent<NetworkSpectator>())
        {
            var spectator = conn.identity.GetComponent<NetworkSpectator>();
            NetworkSpectators.Remove(spectator);
            Destroy(spectator.gameObject);
        }

        ConnectionEvent?.Invoke();

        // ServerChangeScene(lobbyScene);

        base.OnServerConnect(conn);
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        bool isVrUser = PlayerPrefs.GetString("device") == PlayerDevice.VR.ToString();

        GameObject prefab = isVrUser ? playerPrefab : spectatorPrefab.gameObject;
        GameObject player = Instantiate(prefab);
        NetworkServer.AddPlayerForConnection(conn, player);

        ConnectionEvent?.Invoke();
    }
    #endregion

    public void NotifyReadyState()
    {
        ReadyStateEvent?.Invoke(IsReadyToStart());
        Debug.LogWarning("Ready State changed somewhere");
    }

    public bool IsReadyToStart()
    {
        bool team1Ready = false;
        bool team2Ready = false;

        foreach (var player in NetworkPlayers)
        {
            if (player.IsReady && player.Team == PlayerTeam.P1) { team1Ready = true; }
            if (player.IsReady && player.Team == PlayerTeam.P2) { team2Ready = true; }
        }

        return team1Ready && team2Ready;
    }
}

public enum PlayerDevice
{
    VR,
    Tablet
}
