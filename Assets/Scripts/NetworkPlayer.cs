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
    public void StartBriefing(int briefingIndex)
    {
        if (isClient && isLocalPlayer)
        {
            Debug.Log(briefingIndex);

            Briefing briefing = Database.Instance.briefings[briefingIndex];
            PlayerRole rol = id == PlayerId.Player1 ? briefing.spelerRol1 : briefing.spelerRol2;

            StopAllCoroutines();
            StartCoroutine(StopBriefingNaTijd(5.0f));
        }
    }

    public IEnumerator StopBriefingNaTijd(float vertraging)
    {
        // Wacht tot de opgegeven tijd is verstreken en laat
        // de server weten dat de speler gebrieft is
        yield return new WaitForSeconds(vertraging);
        gameManager.CmdFinishBriefing(NetworkServer.localConnection);
    }

    [ClientRpc]
    public void UpdateSpelerIdentiteit(PlayerId id)
    {
        this.id = id;
    }
}

public enum PlayerId
{
    Player1,
    Player2
}
