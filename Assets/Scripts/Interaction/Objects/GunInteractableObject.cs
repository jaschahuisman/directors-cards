using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Mirror;

[RequireComponent(typeof(Rigidbody))]
public class GunInteractableObject : InteractableObject
{
    [SerializeField] private float shootingForce;
    [SerializeField] private float bulletLifeTime;
    [SerializeField] private float recoilForce;
    [SerializeField] private Transform barrelLocation;
    [SerializeField] private Projectile bulletPrefab;

    private Rigidbody rigidBody;

    public override void Start()
    {
        base.Start();

        interactableObject.activated.AddListener(Shoot);
    }

    public void Shoot(ActivateEventArgs args)
    {
        args.interactorObject
            .transform
            .gameObject.GetComponent<XRController>()
                .SendHapticImpulse(1, 0.2f);
        
        Projectile bullet = Instantiate(bulletPrefab, barrelLocation.position, barrelLocation.rotation);
        NetworkServer.Spawn(bullet.gameObject);
        
        bullet.CmdShootBullet(shootingForce, bulletLifeTime);
    }

    public void ApplyRecoil()
    {
        rigidBody.AddRelativeForce(Vector3.back * recoilForce, ForceMode.Impulse);
    }
}
