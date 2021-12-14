using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SpatialTracking;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;
using Mirror;

public class NetworkPlayer : NetworkBehaviour
{
    [Header("Player identitification")]
    public PlayerId id;

    [Header("Speler trackable objects")]
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private GameObject leftController;
    [SerializeField] private GameObject rightController;
    [SerializeField] private bool disabledNonAuthoritatives;

    private NetworkManagerExtended netwerk;
    private GameManager gameManager;

    private void Start()
    {
        netwerk = NetworkManagerExtended.Instance;
        gameManager = GameManager.Instance;
    }

    private void Update()
    {
        if(!isLocalPlayer && !disabledNonAuthoritatives)
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

    [ClientRpc]
    public void RpcStartBriefing(int briefingIndex)
    {
        if (isClient && isLocalPlayer)
        {
            BriefingScriptable briefing = Database.Instance.briefings[briefingIndex];

            BriefingManager.Instance.StartBriefing(briefing, this);

            StopAllCoroutines();
            StartCoroutine(StopBriefingAfterTimelineFinished((float) briefing.timeline.duration + 1));
        }
    }

    public IEnumerator StopBriefingAfterTimelineFinished(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameManager.CmdFinishBriefing(NetworkServer.localConnection);
    }

    [ClientRpc]
    public void RpcReceiveImprovCard(int cardIndex)
    {
        Debug.Log(cardIndex + " recieved from host");
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
