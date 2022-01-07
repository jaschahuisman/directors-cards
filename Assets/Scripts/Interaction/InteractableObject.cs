using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class InteractableObject : MonoBehaviour
{
    [HideInInspector]
    public XRGrabInteractable interactableObject;

    public virtual void Start()
    {
        interactableObject.selectEntered.AddListener(PickupItem);
        interactableObject.selectExited.AddListener(DropItem);
    }
   
    private void PickupItem(SelectEnterEventArgs args)
    {
        args.interactorObject.transform.gameObject.GetComponent<InteractorMeshHider>().HideMesh();
    }

    private void DropItem(SelectExitEventArgs args)
    {
        args.interactorObject.transform.gameObject.GetComponent<InteractorMeshHider>().ShowMesh();
    }
}
