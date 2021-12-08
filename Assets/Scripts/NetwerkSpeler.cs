using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SpatialTracking;
using UnityEngine.XR.Interaction.Toolkit;
using Mirror;

public class NetwerkSpeler : NetworkBehaviour
{
    [Header("Speler trackable objecten")]
    [SerializeField] private GameObject spelersCamera;
    [SerializeField] private GameObject leftController;
    [SerializeField] private GameObject rightController;
    [SerializeField] private bool trackablesUitgeschakeld;

    // De static netwerk instance
    private NetwerkManager netwerk;

    private void Start()
    {
        // Sla de static netwerk instance op
        netwerk = NetwerkManager.Instance;
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
}
