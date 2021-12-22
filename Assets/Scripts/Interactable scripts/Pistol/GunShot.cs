using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(XRGrabInteractable))]

public class GunShot : MonoBehaviour
{

    //public AudioSource sfx;
    
    
    //private HandController handValue;

    //private float fireRate = 0.05f;
    

    [SerializeField] protected float shootingForce;
    [SerializeField] private float recoilForce;
    [SerializeField] protected Transform barrelLocation;
    [SerializeField] private GameObject bulletPrefab;

    private Rigidbody rigidBody;
    private XRGrabInteractable interactableWeapon;

    protected virtual void Awake()
    {
        interactableWeapon = GetComponent<XRGrabInteractable>();
        rigidBody = GetComponent<Rigidbody>();
        SetupInteractableWeaponEvents();
    }

    private void SetupInteractableWeaponEvents()
    {
        interactableWeapon.onSelectEnter.AddListener(PickUpWeapon);
        interactableWeapon.onSelectExit.AddListener(DropWeapon);
        interactableWeapon.onActivate.AddListener(StartShooting);
        interactableWeapon.onDeactivate.AddListener(StopShooting);
    }

    private void PickUpWeapon(XRBaseInteractor interactor)
    {
        interactor.GetComponent<MeshHidder>().Hide();    
    }

    private void DropWeapon(XRBaseInteractor interactor)
    {
        interactor.GetComponent<MeshHidder>().Show();
    }

    protected virtual void StartShooting(XRBaseInteractor interactor)
    {
           Shoot();
    }

    protected virtual void StopShooting(XRBaseInteractor interactor)
    {

    }

    protected virtual void Shoot()
    {
        ApplyRecoil();
        GameObject beamInstance = Instantiate(bulletPrefab, barrelLocation.position, barrelLocation.rotation);
    }

    private void ApplyRecoil()
    {
        rigidBody.AddRelativeForce(Vector3.back * recoilForce, ForceMode.Impulse);
    }

    // public float GetShootingForce() 
    // {
    //     return shootingForce;
    // }




















    // // Start is called before the first frame update
    // void Start()
    // {
    //     handValue = GameObject.FindObjectOfType<HandController>();

    //     if (barrelLocation == null) 
    //     {
    //         barrelLocation = transform;
    //     }
    // }

    // // Update is called once per frame
    // void Update()
    // {
    //      if (handValue.indexValue <= 1f && handValue.indexValue >= 0.9f)
    //      {
    //        Debug.Log("gun fired");
    //      } 

    // }
}
