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

    public static event Action<int> ServerOnPlayerDie;

    #region Server

    public override void OnStartServer()
    {
        health = GetComponent<Health>();
        health.ServerOnDie += ServerHandleDeath;

        ServerOnBaseSpawn?.Invoke(this);

    }

    public override void OnStopServer()
    {

        ServerOnBaseDespawn?.Invoke(this);

        health.ServerOnDie -= ServerHandleDeath;


    }

    [Server]
    private void ServerHandleDeath()
    {
        ServerOnPlayerDie?.Invoke(connectionToClient.connectionId);
        NetworkServer.Destroy(gameObject);
    }


    #endregion






    #region Client



    #endregion




}
