using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    Rigidbody rb;
    [SerializeField] float launchForce = 10f;
    [SerializeField] float destroyAfterSeconds = 5f;
    [SerializeField] int damageToDeal = 20;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * launchForce;
    }

    public override void OnStartServer()
    {
        Invoke(nameof(DestroySelf), destroyAfterSeconds);
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        //does it belong to us?
        if(other.TryGetComponent<NetworkIdentity>(out NetworkIdentity networkIdentity))
        {
            if (networkIdentity.connectionToClient == connectionToClient)
                return;
        }

        //if the collider is damageable (ie has health) then hurt it.
        if (other.TryGetComponent<Health>(out Health health))
        {
            health.DealDamage(damageToDeal);
        }

            //get rid of projectile once it hits anything
            DestroySelf();
    }

    [Server]
    void DestroySelf()
    {
        NetworkServer.Destroy(this.gameObject);
    }



}
