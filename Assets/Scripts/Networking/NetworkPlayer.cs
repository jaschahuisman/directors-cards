using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SpatialTracking;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;
using Mirror;

public class NetworkPlayer : NetworkBehaviour
{
    [Header("Authority-only components")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private TrackedPoseDriver trackedPoseDriver;
    [SerializeField] private AudioListener audioListener;
    [SerializeField] private XRController leftController;
    [SerializeField] private XRController rightController;
    [SerializeField] private GameObject leftLineInteractor;
    [SerializeField] private GameObject rightLineInteractor;
    [SerializeField] private HandController leftHandController;
    [SerializeField] private HandController rightHandController;

    [Header("Game components")]
    [SerializeField] private AudioSource buzzerSound;
    [SerializeField] private GameObject playerCardPrefab;
    [SerializeField] private Transform playerWrist;
    [SerializeField] private Animator notificationAnimator;

    [Header("Values for spectator")]
    public Transform bodyTransform;

    [Header("Values for the head")]
    public Transform headTransform;

    [Header("Status")]
    [SyncVar]
    public PlayerTeam Team;

    [SyncVar(hook = nameof(HandleReadyStatusChanged))]
    public bool IsReady = false;

    public event System.Action<bool> OnReadyChanged; 

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
        leftLineInteractor.SetActive(true);
        rightLineInteractor.SetActive(true);
        leftHandController.enabled = true;
        rightHandController.enabled = true;

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

        ToggleXRRayInteractors(false);
    }

    [Command]
    public void CmdFinishBriefing()
    {
        Network.BriefedPlayers.Add(this);
        Network.NotifyFinishedBriefing();
    }

    [Server]
    public void SpawnHead(int briefingIndex)
    {
        Debug.LogWarning("Spawning a head for player: " + Team + " " + briefingIndex);


        Briefing briefing = Database.Instance.briefings[briefingIndex];
        GameObject headObject = Team == PlayerTeam.P1
            ? briefing.playerRole1.characterModel
            : briefing.playerRole2.characterModel;

        GameObject head = Instantiate(headObject);
        NetworkServer.Spawn(head, gameObject);

        foreach(Transform child in headTransform) { Destroy(child.gameObject); }
        head.transform.SetParent(headTransform);
        head.transform.position = headTransform.position;

        RpcSpawnHead(head);
    }

    [ClientRpc]
    public void RpcSpawnHead(GameObject head)
    {
        foreach (Transform child in headTransform) { Destroy(child.gameObject); }
        head.transform.SetParent(headTransform);
        head.transform.position = headTransform.position;
    }

    [ClientRpc]
    public void RpcReceiveCard(int cardIndex)
    {
        if (isLocalPlayer)
        {
            Card card = Database.Instance.cards[cardIndex];

            SendHaptics();
            buzzerSound.Play();
            notificationAnimator.SetTrigger("PlayAnimation");

            foreach (Transform child in playerWrist) { Destroy(child.gameObject); }

            GameObject playerCardObject = Instantiate(playerCardPrefab);
            PlayerCard playerCard = playerCardObject.GetComponent<PlayerCard>();

            playerCard.SetData(card, Team);

            playerCardObject.transform.SetParent(playerWrist, false);
            playerCardObject.SetActive(true);

            Debug.LogWarning("Received card with index " + cardIndex);
        }
    }

    private void SendHaptics()
    {
        if (leftController != null) { leftController.SendHapticImpulse(1f, 0.6f); }
    }

    public void HandleReadyStatusChanged(bool oldVlaue, bool newValue)
    {
        OnReadyChanged?.Invoke(newValue);

        if (!newValue)
        {
            foreach (Transform child in playerWrist) { Destroy(child.gameObject); }
        }
    }

    public void ToggleXRRayInteractors(bool value)
    {
        leftLineInteractor.SetActive(value);
        rightLineInteractor.SetActive(value);
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
