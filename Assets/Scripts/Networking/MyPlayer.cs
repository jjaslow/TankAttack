﻿using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayer : NetworkBehaviour
{
    List<Unit> myUnits = new List<Unit>();
    List<Building> myBuildings = new List<Building>();

    #region Getters
    public List<Unit> GetMyUnits()
    {
        return myUnits;
    }
    public List<Building> GetMyBuildings()
    {
        return myBuildings;
    }
    #endregion

    #region Server
    public override void OnStartServer()
    {
        Unit.ServerOnUnitSpawned += ServerAddUnitToList;
        Unit.ServerOnUnitDespawned += ServerRemoveUnitFromList;
        Building.ServerOnBuildingSpawned += ServerAddBuildingToList;
        Building.ServerOnBuildingDespawned += ServerRemoveBuildingFromList;
    }


    public override void OnStopServer()
    {
        Unit.ServerOnUnitSpawned -= ServerAddUnitToList;
        Unit.ServerOnUnitDespawned -= ServerRemoveUnitFromList;
        Building.ServerOnBuildingSpawned -= ServerAddBuildingToList;
        Building.ServerOnBuildingDespawned -= ServerRemoveBuildingFromList;
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


    private void ServerAddBuildingToList(Building building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId)
            return;

        myBuildings.Add(building);
    }

    private void ServerRemoveBuildingFromList(Building building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId)
            return;

        myBuildings.Remove(building);
    }

    #endregion


    #region Client

    public override void OnStartAuthority()
    {
        if (NetworkServer.active)
            return;

        Unit.AuthorityOnUnitSpawned += AuthorityAddUnitToList;
        Unit.AuthorityOnUnitDespawned += AuthorityRemoveUnitFromList;
        Building.AuthorityOnBuildingSpawned += AuthorityAddBuildingToList;
        Building.AuthorityOnBuildingDespawned += AuthorityRemoveBuildingFromList;
    }


    public override void OnStopClient()
    {
        if (!isClientOnly || !hasAuthority)
            return;

        Unit.AuthorityOnUnitSpawned -= AuthorityAddUnitToList;
        Unit.AuthorityOnUnitDespawned -= AuthorityRemoveUnitFromList;
        Building.AuthorityOnBuildingSpawned -= AuthorityAddBuildingToList;
        Building.AuthorityOnBuildingDespawned -= AuthorityRemoveBuildingFromList;
    }

    private void AuthorityAddUnitToList(Unit unit)
    {
        myUnits.Add(unit);
    }

    private void AuthorityRemoveUnitFromList(Unit unit)
    {
        myUnits.Remove(unit);
    }


    private void AuthorityRemoveBuildingFromList(Building building)
    {
        myBuildings.Add(building);
    }

    private void AuthorityAddBuildingToList(Building building)
    {
        myBuildings.Remove(building);
    }


    #endregion




}

