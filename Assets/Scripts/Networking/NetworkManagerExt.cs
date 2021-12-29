using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.Discovery;
using UnityEngine.SceneManagement;


public class NetworkManagerExt : NetworkManager
{
    [Header("Prefabs")]
    [SerializeField] private GameObject spectatorPrefab;

    [Header("Gameplay")]
    [Scene] [SerializeField] private string gameplayScene = string.Empty;

    public List<NetworkPlayer> NetworkPlayers = new List<NetworkPlayer>();
    public List<NetworkSpectator> NetworkSpectators = new List<NetworkSpectator>();

    public List<NetworkPlayer> GamePlayers = new List<NetworkPlayer>();
    public List<NetworkPlayer> BriefedPlayers = new List<NetworkPlayer>();

    public static event Action<bool> GameplayReadyEvent;
    public static event Action<bool> GameplayStartedEvent;
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
            NotifyReadyToLoadGameplay();
            Destroy(player.gameObject);
        }

        if (conn.identity != null && conn.identity.GetComponent<NetworkSpectator>())
        {
            var spectator = conn.identity.GetComponent<NetworkSpectator>();
            NetworkSpectators.Remove(spectator);
            Destroy(spectator.gameObject);
        }

        ConnectionEvent?.Invoke();

        Scene activeScene = SceneManager.GetActiveScene();

        if (!onlineScene.Contains(activeScene.name))
        {
            ServerChangeScene(onlineScene);
        }

        base.OnServerConnect(conn);
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        bool isVrUser = PlayerPrefs.GetString("device") == PlayerDevice.VR.ToString();

        Debug.Log(isVrUser ? "Adding vr user" : "Adding spectator");

        GameObject prefab = isVrUser ? playerPrefab : spectatorPrefab.gameObject;
        GameObject player = Instantiate(prefab);
        NetworkServer.AddPlayerForConnection(conn, player);

        ConnectionEvent?.Invoke();
    }
    #endregion

    #region Client
    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        base.OnClientSceneChanged(conn);

        if(!conn.identity) { return; }

        Scene activeScene = SceneManager.GetActiveScene();
        NetworkPlayer player = conn.identity.GetComponent<NetworkPlayer>();

        if (!player) { return; }


        if (gameplayScene.Contains(activeScene.name))
        {
            player.CmdReadyInGameplayScene(); 
        }

        if (onlineScene.Contains(activeScene.name))
        {
            player.ToggleXRRayInteractors(true);
        }
    }
    #endregion

    #region State management
    public void NotifyReadyToLoadGameplay()
    {
        GameplayReadyEvent?.Invoke(IsReadyToLoadGameplay());
        Debug.LogWarning("Gameplay is ready to be loaded!");
    }

    public void NotifyReadyToStartBriefing()
    {
        if (IsReadyToStartBriefing())
        {
            Debug.LogWarning("Briefing is ready to start!");
            StartBriefing();
        }
    }

    public void NotifyFinishedBriefing()
    {
        if (IsFinishedBriefing())
        {
            StartGameplay();
        }
    }

    public bool IsReadyToLoadGameplay()
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

    public bool IsReadyToStartBriefing()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        return gameplayScene.Contains(activeScene.name) && GamePlayers.Count == NetworkPlayers.Count;
    }

    public bool IsFinishedBriefing()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        return gameplayScene.Contains(activeScene.name) && BriefedPlayers.Count == NetworkPlayers.Count;
    }
    #endregion

    public void LoadGameplayScene()
    {
        ServerChangeScene(gameplayScene);
    }

    public void StartBriefing()
    {
        BriefedPlayers.Clear();
        int briefingIndex = UnityEngine.Random.Range(0, Database.Instance.briefings.Count);

        foreach(var player in GamePlayers)
        {
            player.RpcStartBriefing(briefingIndex);
        }
    }

    public void StartGameplay()
    {
        GameplayStartedEvent?.Invoke(true);

        Debug.LogWarning("Gameplay Started after briefing all players");
    }

    public void StopGameplay()
    {
        GameplayStartedEvent?.Invoke(false);

        GamePlayers.Clear();

        foreach(var player in NetworkPlayers)
        {
            player.IsReady = false;
        }

        ServerChangeScene(onlineScene);
    }

    public void SendCardToClient(int cardIndex, PlayerTeam team)
    {
        foreach (var player in NetworkPlayers)
        {
            if (player.Team == team) player.RpcReceiveCard(cardIndex);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LoadGameplayScene();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            StopGameplay();
        }
    }
}

public enum PlayerDevice
{
    VR,
    Tablet
}
