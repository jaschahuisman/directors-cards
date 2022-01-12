using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Mirror;

[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(NetworkIdentity))]
public class InteractableObject : NetworkBehaviour
{
    [HideInInspector]
    public XRGrabInteractable interactableObject;

    [HideInInspector]
    public NetworkIdentity networkIdentity;

    public override void OnStartServer()
    {
        Setup();
    }

    public override void OnStartClient()
    {
        Setup();
    }

    private void Setup()
    {
        interactableObject = gameObject.GetComponent<XRGrabInteractable>();
        networkIdentity = gameObject.GetComponent<NetworkIdentity>();
        interactableObject.onSelectEntered.AddListener(PickupItem);
        interactableObject.onSelectExited.AddListener(DropItem);
    }

    private void PickupItem(XRBaseInteractor interactor)
    {
        Debug.Log(interactor);

        CmdAssignAuthority();
        if (interactor != null)
        {
            interactor.GetComponent<InteractorMeshHider>().HideMesh();
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdAssignAuthority(NetworkConnectionToClient conn = null)
    {
        networkIdentity.AssignClientAuthority(conn);
    }

    private void DropItem(XRBaseInteractor interactor)
    {
        if (interactor != null)
        {
            interactor.GetComponent<InteractorMeshHider>().ShowMesh();
        }
    }
}
