using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TEMPHider : MonoBehaviour
{

    private XRGrabInteractable Interactable;

    void Awake()
    {
        Interactable = GetComponent<XRGrabInteractable>();

        SetupInteractableEvents();
    }

    private void SetupInteractableEvents()
    {
        Interactable.onSelectEntered.AddListener(PickUpObject);
        Interactable.onSelectExited.AddListener(DropObject);

    }

    private void PickUpObject(XRBaseInteractor interactor)
    {
        interactor.GetComponent<MeshHidder>().Hide();
    }

    private void DropObject(XRBaseInteractor interactor)
    {
        interactor.GetComponent<MeshHidder>().Show();
    }
}
