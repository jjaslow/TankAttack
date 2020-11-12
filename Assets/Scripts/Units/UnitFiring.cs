using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitFiring : NetworkBehaviour
{

    Targeter targeter;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform projectileSpawPoint;
    [SerializeField] float fireRange = 5;
    [SerializeField] float fireRate = 1;
    [SerializeField] float rotationSpeed = 20f;

    float lastFireTime;


    [ServerCallback]
    private void Start()
    {
        targeter = GetComponent<Targeter>();
    }


    [ServerCallback]
    private void Update()
    {
        if (targeter.GetTarget() == null)
            return;

        if (!CanFireAtTarget())
            return;

        Quaternion targetRotation = Quaternion.LookRotation(targeter.GetTarget().transform.position - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        if(Time.time > (1/fireRate) + lastFireTime)
        {
            lastFireTime = Time.time;

            Quaternion projectileRotation = Quaternion.LookRotation(targeter.GetTarget().GetAimAtPoint().position - projectileSpawPoint.position);
            GameObject projectileInstance = Instantiate(projectilePrefab, projectileSpawPoint.position, projectileRotation);
            NetworkServer.Spawn(projectileInstance, connectionToClient);
        }
    }


    [Server]
    private bool CanFireAtTarget()
    {
        if (Vector3.Distance(transform.position, targeter.GetTarget().transform.position) > fireRange)
            return false;



        return true;
    }

}
