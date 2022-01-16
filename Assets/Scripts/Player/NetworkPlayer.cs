using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public enum PlayerType { Player1, Player2 }

public class NetworkPlayer : NetworkBehaviour
{
    [Header("Authority (make sure to disable on player)")]
    [SerializeField] private List<Behaviour> authoritativeBehaviours = new List<Behaviour>();
    [SerializeField] private List<GameObject> authoritativeGameObjects = new List<GameObject>();

    [Header("Card related components")]
    [SerializeField] public PlayerGameplayManager playerGameplayManager;

    [Header("Network Player Status")]
    [SyncVar(hook = nameof(HandleTeamChanged))]
    public PlayerType team = PlayerType.Player1;

    [SyncVar(hook = nameof(HandleReadyStateChanged))]
    public bool isReady = false;

    public static event System.Action<bool> OnReadyStateChanged;

    private NetworkManagerExtended network;
    private NetworkManagerExtended Network
    {
        get
        {
            if (network != null) { return network; }
            return network = NetworkManagerExtended.singleton as NetworkManagerExtended;
        }
    }

    public override void OnStartAuthority()
    {
        foreach (Behaviour behaviour in authoritativeBehaviours)
        {
            behaviour.enabled = true;
        }

        foreach (GameObject gameObject in authoritativeGameObjects)
            gameObject.SetActive(true);

        base.OnStartAuthority();
    }

    public override void OnStartClient() 
    {
        OnReadyStateChanged?.Invoke(isReady);
        SetupPlayerInNetwork(); 
    }

    public override void OnStartServer() 
    { 
        if(isServerOnly) 
            SetupPlayerInNetwork(); 
    }

    public override void OnStopClient() 
    {
        OnReadyStateChanged?.Invoke(isReady);
        RemovePlayerFromNetwork(); 
    }

    public override void OnStopServer() 
    { 
        if (isServerOnly)
            RemovePlayerFromNetwork();
    }

    private void SetupPlayerInNetwork()
    {
        Network.networkPlayers.Add(this);
        DontDestroyOnLoad(gameObject);
    }

    private void RemovePlayerFromNetwork()
    {
        Network.networkPlayers.Remove(this);
        Network.briefedPlayers.Remove(this);
        Network.playingPlayers.Remove(this);
    }

    public void HandleReadyStateChanged(bool oldVlaue, bool newReadyState)
    {
        OnReadyStateChanged?.Invoke(newReadyState);

        if (newReadyState == false)
            playerGameplayManager.DestroyCards();
    }

    public void HandleTeamChanged(PlayerType oldValue, PlayerType newValue)
    {
        OnReadyStateChanged?.Invoke(isReady);
    }

    [Command]
    public void CmdSetPlayerTeam(PlayerType newTeam) { team = newTeam; }

    [Command]
    public void CmdSetReadyState(bool newReadyState)
    {
        isReady = newReadyState;
        Network.HandlePlayerReadyStateChange();
    }

    [Command]
    public void CmdReadyInGameplayScene()
    {
        Network.playingPlayers.Add(this);
        Network.HandlePlayerGameplaySceneLoaded();
    }

    [ClientRpc]
    public void RpcStartBriefing(int briefingIndex)
    {
        var briefing = Database.Instance.briefings[briefingIndex];
        TheatreSceneManager.Instance.StartBriefing(briefing, this);
    }

    public void FinishBriefing()
    {
        if (isLocalPlayer) CmdFinishPlayerBriefing();
    }

    [Command]
    public void CmdFinishPlayerBriefing()
    {
        Network.briefedPlayers.Add(this);
        Network.HandlePlayerBriefingFinished();
    }

    [ClientRpc]
    public void RpcFinishScene()
    {
        TheatreSceneManager.Instance.FinishScene(this);
    }

    public void FinishPlayerSceneDone()
    {
        if (isLocalPlayer) CmdFinishPlayerSceneDone();
    }

    [Command]
    public void CmdFinishPlayerSceneDone()
    {
        Network.finishedPlayers.Add(this);
        Network.HandlePlayerFinishedScene();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            CmdSetReadyState(true);
        }
    }
}

