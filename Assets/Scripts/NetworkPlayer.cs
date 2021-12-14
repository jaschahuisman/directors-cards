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
    [Header("Player identitification")]
    public PlayerId id;

    [Header("Player trackable objects")]
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private GameObject leftController;
    [SerializeField] private GameObject rightController;
    [SerializeField] private bool disabledNonAuthoritatives;

    [Header("Player UI")]
    [SerializeField] private XRBaseController xrBaseController;
    [SerializeField] private GameObject improvCardPrefab;
    [SerializeField] private Transform playerWrist;
    [SerializeField] private AudioClip buzzerSound;

    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
        GameManager.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= OnGameStateChanged;
    }

    private void Update()
    {
        if (!isLocalPlayer && !disabledNonAuthoritatives)
        {
            gameObject.GetComponent<XROrigin>().enabled = false;
            gameObject.GetComponent<AudioSource>().enabled = false;

            playerCamera.GetComponent<Camera>().enabled = false;
            playerCamera.GetComponent<AudioListener>().enabled = false;
            playerCamera.GetComponent<TrackedPoseDriver>().enabled = false;

            leftController.GetComponent<XRController>().enabled = false;
            rightController.GetComponent<XRController>().enabled = false;

            disabledNonAuthoritatives = true;
        }
    }

    private void OnGameStateChanged(GameState newGameState)
    {
        if(newGameState == GameState.WaitingForHost)
        {
            foreach (Transform child in playerWrist) Destroy(child.gameObject);
        }
    }

    [ClientRpc]
    public void RpcStartBriefing(int briefingIndex)
    {
        if (isClient && isLocalPlayer)
        {
            BriefingScriptable briefing = Database.Instance.briefings[briefingIndex];

            BriefingManager.Instance.StartBriefing(briefing, this);

            StopAllCoroutines();
            StartCoroutine(StopBriefingAfterTimelineFinished((float)briefing.timeline.duration + 1));
        }
    }

    public IEnumerator StopBriefingAfterTimelineFinished(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameManager.CmdFinishBriefing(NetworkServer.localConnection);
    }

    [ClientRpc]
    public void RpcReceiveImprovCard(int cardIndex, PlayerId playerId)
    {
        if (id == playerId && isLocalPlayer)
        {
            // Debug.Log(cardIndex + " recieved from host " + playerId + " " + id);

            if (xrBaseController != null) xrBaseController.SendHapticImpulse(1f, 0.25f);
            gameObject.GetComponent<AudioSource>().clip = buzzerSound;
            gameObject.GetComponent<AudioSource>().Play();

            foreach (Transform child in playerWrist) Destroy(child.gameObject);

            GameObject cardObject = Instantiate(improvCardPrefab);
            PlayerImprovCard playerImprovCard = cardObject.GetComponent<PlayerImprovCard>();

            playerImprovCard.SetData(cardIndex, playerId);
            cardObject.gameObject.transform.SetParent(playerWrist, false);
        }
    }

    [ClientRpc]
    public void RpcUpdatePlayerId(PlayerId newId)
    {
        this.id = newId;
    }
}

public enum PlayerId
{
    Player1,
    Player2
}
