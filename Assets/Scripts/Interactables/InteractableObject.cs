using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Mirror;
using System;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NetworkIdentity))]
[RequireComponent(typeof(NetworkTransform))]
[RequireComponent(typeof(XRGrabInteractable))]
// [RequireComponent(typeof(Mirror.Experimental.NetworkRigidbody))]
public class InteractableObject : NetworkBehaviour
{
    [HideInInspector] public NetworkIdentity networkIdentity;
    [HideInInspector] public XRGrabInteractable grabInteractable;
    [SyncVar] public NetworkIdentity owner;
    
    private AudioSource audioSource;

    public override void OnStartServer()
    {
        base.OnStartServer();
        Setup();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (isClientOnly)
            Setup();
    }

    private void Setup()
    {
        audioSource = GetComponent<AudioSource>();

        networkIdentity = GetComponent<NetworkIdentity>();
        grabInteractable = GetComponent<XRGrabInteractable>();

        GetComponent<NetworkTransform>().clientAuthority = true;

        grabInteractable.onSelectEntered.AddListener(PickupItem);
    }

    private void PickupItem(XRBaseInteractor interactor)
    {
        if (audioSource != null)
        {
            audioSource.Play();
        }
        CmdAssignAuthority();
    }

    [Command(requiresAuthority = false)]
    private void CmdAssignAuthority(LocalConnectionToClient conn = null)
    {
        networkIdentity.RemoveClientAuthority();
        networkIdentity.AssignClientAuthority(conn);
        owner = conn.identity;
    }
}
