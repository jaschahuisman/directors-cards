using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SpatialTracking;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;
using Mirror;

public class NetwerkSpeler : NetworkBehaviour
{
    [Header("Spelers identiteit")]
    public SpelerId spelerId;

    [Header("Speler trackable objecten")]
    [SerializeField] private GameObject spelersCamera;
    [SerializeField] private GameObject leftController;
    [SerializeField] private GameObject rightController;
    [SerializeField] private bool trackablesUitgeschakeld;

    // De static netwerk instance
    private NetwerkManager netwerk;
    private GameManager gameManager;

    private void Start()
    {
        // Sla de static netwerk instance op
        netwerk = NetwerkManager.Instance;
        gameManager = GameManager.Instance;

        // Luister naar veranderende game status
        GameManager.OnGameStatusChanged += OnGameStatusChanged;
    }

    private void Update()
    {
        // Schakel de trackables (camera + controllers) uit wanneer
        // de verbonden speler niet over deze netwerkspeler heerst.
        if(!isLocalPlayer && !trackablesUitgeschakeld)
        {
            // Disable XR origin
            gameObject.GetComponent<XROrigin>().enabled = false;

            // Disable camera components that are not belonging to the player
            spelersCamera.GetComponent<Camera>().enabled = false;
            spelersCamera.GetComponent<AudioListener>().enabled = false;
            spelersCamera.GetComponent<TrackedPoseDriver>().enabled = false;

            // Disable controller components that are not belonging to the player
            leftController.GetComponent<XRController>().enabled = false;
            rightController.GetComponent<XRController>().enabled = false;

            // Set disabled state
            trackablesUitgeschakeld = true;
        }
    }

    private void OnGameStatusChanged(GameStatus nieuweStatus)
    {
        if (isClient && nieuweStatus == GameStatus.Briefing)
        {
            // Start de briefing
            StartBriefing();
        }
    }

    public void StartBriefing()
    {
        if (isClient)
        {
            // Wacht tot de opgegeven tijd is verstreken en laat
            // de server weten dat de speler gebrieft is
            StartCoroutine(StopBriefingNaTijd(5.0f));
        }
    }

    public IEnumerator StopBriefingNaTijd(float vertraging)
    {
        // Wacht tot de opgegeven tijd is verstreken en laat
        // de server weten dat de speler gebrieft is
        yield return new WaitForSeconds(vertraging);
        gameManager.SpelerBriefingAfgelopen(NetworkServer.localConnection);
    }

    [ClientRpc]
    public void UpdateSpelerIdentiteit(SpelerId id)
    {
        spelerId = id;
    }
}

public enum SpelerId
{
    Speler1,
    Speler2
}
