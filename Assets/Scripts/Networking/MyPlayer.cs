using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayer : NetworkBehaviour
{
    [SerializeField]
    List<Unit> myUnits = new List<Unit>();

    public List<Unit> GetMyUnits()
    {
        return myUnits;
    }



    #region Server
    public override void OnStartServer()
    {
        Unit.ServerOnUnitSpawned += ServerAddUnitToList;
        Unit.ServerOnUnitDespawned += ServerRemoveUnitFromList;
    }


    public override void OnStopServer()
    {
        Unit.ServerOnUnitSpawned -= ServerAddUnitToList;
        Unit.ServerOnUnitDespawned -= ServerRemoveUnitFromList;
    }

    private void ServerAddUnitToList(Unit unit)
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId)
            return;

        myUnits.Add(unit);
    }

    private void ServerRemoveUnitFromList(Unit unit)
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId)
            return;

        myUnits.Remove(unit);
    }

    #endregion


    #region Client

    public override void OnStartAuthority()
    {
        if (NetworkServer.active)
            return;

        Unit.AuthorityOnUnitSpawned += AuthorityAddUnitToList;
        Unit.AuthorityOnUnitDespawned += AuthorityRemoveUnitFromList;
    }


    public override void OnStopClient()
    {
        if (!isClientOnly || !hasAuthority)
            return;

        Unit.AuthorityOnUnitSpawned -= AuthorityAddUnitToList;
        Unit.AuthorityOnUnitDespawned -= AuthorityRemoveUnitFromList;
    }

    private void AuthorityAddUnitToList(Unit unit)
    {
        myUnits.Add(unit);
    }

    private void AuthorityRemoveUnitFromList(Unit unit)
    {
        myUnits.Remove(unit);
    }



    #endregion




}

