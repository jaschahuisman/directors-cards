using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SpatialTracking;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;
using Unity.Audio;
using Mirror;

public class NetworkPlayer : NetworkBehaviour
{
    [Header("Authority-only components")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private TrackedPoseDriver trackedPoseDriver;
    [SerializeField] private AudioListener audioListener;
    [SerializeField] private XRController leftController;
    [SerializeField] private XRController rightController;

    [Header("Game components")]
    [SerializeField] private GameObject playerCardPrefab;

    [Header("Status")]
    [SyncVar]
    public PlayerTeam Team;

    [SyncVar(hook = nameof(HandleReadyStatusChanged))]
    public bool IsReady = false;

    private NetworkManagerExt network;
    private NetworkManagerExt Network
    {
        get
        {
            if (network != null) { return network; }
            return network = NetworkManagerExt.singleton as NetworkManagerExt;
        }
    }

    public override void OnStartAuthority()
    {
        playerCamera.enabled = true;
        trackedPoseDriver.enabled = true;
        audioListener.enabled = true;
        leftController.enabled = true;
        rightController.enabled = true;

        base.OnStartAuthority();
    }

    public override void OnStartClient()
    {
        Network.NetworkPlayers.Add(this);
        DontDestroyOnLoad(gameObject);
    }

    public override void OnStartServer()
    {
        Network.NetworkPlayers.Add(this);
        DontDestroyOnLoad(gameObject);
    }

    public override void OnStopClient()
    {
        Network.NetworkPlayers.Remove(this);
    }

    public override void OnStopServer()
    {
        Network.NetworkPlayers.Remove(this);

        if (Network.GamePlayers.Contains(this))
        {
            Network.GamePlayers.Remove(this);
        }
    }

    [Command]
    public void CmdSetPlayerTeam(PlayerTeam team)
    {
        Team = team;
    }

    [Command]
    public void CmdSetReadyState(bool value)
    {
        IsReady = value;
        Network.NotifyReadyToLoadGameplay();
    }

    [Command]
    public void CmdReadyInGameplayScene()
    {
        Network.GamePlayers.Add(this);
        Network.NotifyReadyToStartBriefing();
    }

    [ClientRpc]
    public void RpcStartBriefing(int briefingIndex)
    {
        Briefing briefing = Database.Instance.briefings[briefingIndex];
        Debug.Log(briefing.name);
        Debug.Log(briefing.playerRole1);
        BriefingManager.Instance.StartBriefing(briefing, this);
    }

    [Command]
    public void CmdFinishBriefing()
    {
        Network.BriefedPlayers.Add(this);
        Network.NotifyFinishedBriefing();
    }

    [ClientRpc]
    public void RpcReceiveCard(int cardIndex)
    {
        Debug.LogWarning("Received card with index " + cardIndex);
    }

    public void HandleReadyStatusChanged(bool oldVlaue, bool newValue)
    {
        ToggleXRRayInteractors(newValue);
        CmdToggleRayInteractors(newValue);
    }

    private void ToggleXRRayInteractors(bool value)
    {
        leftController.GetComponent<LineRenderer>().enabled = !value;
        rightController.GetComponent<LineRenderer>().enabled = !value;

        leftController.GetComponent<XRRayInteractor>().enabled = !value;
        rightController.GetComponent<XRRayInteractor>().enabled = !value;

        leftController.GetComponent<XRInteractorLineVisual>().enabled = !value;
        rightController.GetComponent<XRInteractorLineVisual>().enabled = !value;
    }

    [Command]
    private void CmdToggleRayInteractors(bool value)
    {
        ToggleXRRayInteractors(value);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.LogWarning("Readying Up");
            CmdSetReadyState(true);
        }
    }
}

public enum PlayerTeam
{
    P1,
    P2
}
