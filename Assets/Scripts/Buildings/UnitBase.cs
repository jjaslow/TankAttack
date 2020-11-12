using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBase : NetworkBehaviour
{
    Health health;

    public static event Action<UnitBase> ServerOnBaseSpawn;
    public static event Action<UnitBase> ServerOnBaseDespawn;

    #region Server

    public override void OnStartServer()
    {
        base.OnStartServer();

        health = GetComponent<Health>();
        health.ServerOnDie += ServerHandleDeath;

        ServerOnBaseSpawn?.Invoke(this);
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        health.ServerOnDie -= ServerHandleDeath;

        ServerOnBaseDespawn?.Invoke(this);
    }

    [Server]
    private void ServerHandleDeath()
    {
        NetworkServer.Destroy(gameObject);
    }


    #endregion






    #region Client



    #endregion




}
