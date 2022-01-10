using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class Projectile : MonoBehaviour
{

    protected GunShot weapon;
    [SerializeField] private float lifeTime;
    private Rigidbody rigidBody;

    private void Awake() 
    {
        rigidBody = GetComponent<Rigidbody>();
    }


    public void Init (GunShot weapon){
        this.weapon = weapon;
        Destroy(gameObject, lifeTime);
    }

    public void Launch() 
    {
        rigidBody.AddRelativeForce(Vector3.forward * weapon.GetShootingForce(), ForceMode.Impulse);
    }


}
