﻿using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Unit : NetworkBehaviour
{
    Health health;

    [SerializeField] UnityEvent onSelected = null;
    [SerializeField] UnityEvent onDeselected = null;


    public static event Action<Unit> ServerOnUnitSpawned;
    public static event Action<Unit> ServerOnUnitDespawned;
    public static event Action<Unit> AuthorityOnUnitSpawned;
    public static event Action<Unit> AuthorityOnUnitDespawned;


    #region Server

    public override void OnStartServer()
    {
        ServerOnUnitSpawned?.Invoke(this);

        health = GetComponent<Health>();
        health.ServerOnDie += ServerHandleDie;
    }

    public override void OnStopServer()
    {
        ServerOnUnitDespawned?.Invoke(this);

        health.ServerOnDie -= ServerHandleDie;
    }

    [Server]
    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }


    #endregion




    #region Client
    //Server doesnt need to know which is selected. 
    //Locally we will send the appropriate command from the selected object.
    [Client]
    public void Select()
    {
        if (!hasAuthority)
            return;

        onSelected?.Invoke();

    }

    [Client]
    public void Deselect()
    {
        if (!hasAuthority)
            return;

        onDeselected?.Invoke();

    }


    public override void OnStartAuthority()
    {
        AuthorityOnUnitSpawned?.Invoke(this);
    }

    public override void OnStopClient()
    {
        if (!hasAuthority)
            return;

        AuthorityOnUnitDespawned?.Invoke(this);
    }


    #endregion

}
