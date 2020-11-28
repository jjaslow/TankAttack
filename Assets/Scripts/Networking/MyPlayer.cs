using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MyPlayer : NetworkBehaviour
{
    [SerializeField] Building[] buildings = new Building[0];
    [SerializeField] Transform cameraTransform;

    List<Unit> myUnits = new List<Unit>();
    List<Building> myBuildings = new List<Building>();
    Color myColor;

    [SyncVar(hook = nameof(ClientHandleResourcesUpdated))]
    int resources = 500;
    [SyncVar(hook =nameof(AuthorityHandlePartyOwnerStateUpdated))]
    bool isPartyOwner = false;

    public event Action<int> ClientOnResourcesUpdated;
    public static event Action<bool> AuthorityOnPartyOwnerStateUpdated;

    [SerializeField] LayerMask buildingBlockLayer = new LayerMask();
    [SerializeField] float buildingRangeLimit = 5f;

    #region Getters
    public List<Unit> GetMyUnits()
    {
        return myUnits;
    }
    public List<Building> GetMyBuildings()
    {
        return myBuildings;
    }
    public int GetResources()
    {
        return resources;
    }
    public Color GetMyColor()
    {
        return myColor;
    }
    public Transform GetCameraTransform()
    {
        return cameraTransform;
    }
    public bool GetIsPartyOwner()
    {
        return isPartyOwner;
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


    [Command]
    public void CmdTryPlaceBuilding(int buildingID, Vector3 position)
    {
        Building buildingToPlace = buildings.FirstOrDefault(b => b.GetID() == buildingID);
        if (buildingToPlace == null)
            return;

        //check funds
        if (resources < buildingToPlace.GetPrice())
            return;

        //check placement
        if(!CanPlaceBuildingHere(buildingToPlace, position))
            return;

        GameObject buildingInstance = 
            Instantiate(buildingToPlace.gameObject, position, Quaternion.identity);
        NetworkServer.Spawn(buildingInstance, connectionToClient);

        SetResources(resources - buildingToPlace.GetPrice());
    }

    [Command]
    public void CmdStartGame()
    {
        if (!isPartyOwner) return;

        ((MyNetworkManager)NetworkManager.singleton).StartGame();
    }

    [Server]
    public void SetResources(int newResources)
    {
        resources = newResources;
    }

    [Server]
    public void SetMyColor(Color color)
    {
        myColor = color;
    }

    [Server]
    public void SetPartyOwner(bool state)
    {
        isPartyOwner = state;
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

    public override void OnStartClient()
    {
        if (NetworkServer.active) return;

        ((MyNetworkManager)NetworkManager.singleton).Players.Add(this);
    }


    public override void OnStopClient()
    {
        if (!isClientOnly)
            return;

        ((MyNetworkManager)NetworkManager.singleton).Players.Remove(this);

        if (!hasAuthority) return;

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


    private void AuthorityAddBuildingToList(Building building)
    {
        myBuildings.Add(building);
    }

    private void AuthorityRemoveBuildingFromList(Building building)
    {
        myBuildings.Remove(building);
    }

    void AuthorityHandlePartyOwnerStateUpdated(bool oldState, bool newState)
    {
        if (!hasAuthority) return;

        AuthorityOnPartyOwnerStateUpdated?.Invoke(newState);
    }

    private void ClientHandleResourcesUpdated(int oldResources, int newResources)
    {
        ClientOnResourcesUpdated?.Invoke(newResources);
    }



    #endregion


    public bool CanPlaceBuildingHere(Building buildingToPlace, Vector3 position)
    {
        //check hitting other items
        BoxCollider buildingCollider = buildingToPlace.GetComponent<BoxCollider>();
        if (Physics.CheckBox(position + buildingCollider.center, buildingCollider.size / 2, Quaternion.identity, buildingBlockLayer))
            return false;


        //check in range of other buildings
        foreach (Building building in myBuildings)
        {
            if (Vector3.Distance(position, building.transform.position) <= buildingRangeLimit)
                return true;
        }

        return false;
    }

}

