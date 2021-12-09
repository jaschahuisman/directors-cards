using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SpatialTracking;
using UnityEngine.XR.Interaction.Toolkit;
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

    [Header("Speler audio source")]
    [SerializeField] private AudioSource audioSource;

    // De static netwerk instance
    private NetwerkManager netwerk;
    private TheatersportDatabase database;
    private GameManager gameManager;


    private void Start()
    {
        // Verkrijg de audiosource
        audioSource = GetComponent<AudioSource>();

        // Sla de static manager-instances op
        netwerk = NetwerkManager.Instance;
        database = TheatersportDatabase.Instance;
        gameManager = GameManager.Instance;
    }

    private void Update()
    {
        // Schakel de trackables (camera + controllers) uit wanneer
        // de verbonden speler niet over deze netwerkspeler heerst.
        if(!isLocalPlayer && !trackablesUitgeschakeld)
        {
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

    [ClientRpc]
    public void UpdateSpelerIdentiteit(SpelerId nieuweSpelerId)
    {
        spelerId = nieuweSpelerId;
    }

    [ClientRpc]
    public void StartBriefing(int briefingIndex) {
        TheatersportBriefing briefing = database.briefings[briefingIndex];
        AudioClip speler1Audio = briefing.spelerRol1.briefingAudio;
        AudioClip speler2Audio = briefing.spelerRol2.briefingAudio;

        if (isLocalPlayer)
        {
            // Speel de clip van de juiste speler op het juiste device af
            audioSource.clip = spelerId == SpelerId.Speler1 ? speler1Audio : speler2Audio;
            audioSource.Play();
        }
    }
}

public enum SpelerId
{
    Speler1,
    Speler2
}
