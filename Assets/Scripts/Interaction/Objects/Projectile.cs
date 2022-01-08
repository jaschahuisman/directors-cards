using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : NetworkBehaviour
{
    private Rigidbody rigidBody;

    [Command]
    public void CmdShootBullet(float shootingForce, float lifeTime)
    {
        RpcShootBullet(shootingForce);
        rigidBody.AddRelativeForce(Vector3.forward * shootingForce, ForceMode.Impulse);
        StartCoroutine(DestroyAfterTime(lifeTime));
    }

    [ClientRpc]
    public void RpcShootBullet(float shootingForce)
    {
        rigidBody.AddRelativeForce(Vector3.forward * shootingForce, ForceMode.Impulse);
    }

    public IEnumerator DestroyAfterTime(float delay)
    {
        yield return new WaitForSeconds(delay);
        NetworkServer.Destroy(gameObject);
    }
}
