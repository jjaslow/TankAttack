using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Building : NetworkBehaviour
{
    [SerializeField] GameObject buildingPreview;
    [SerializeField] Sprite icon = null;
    [SerializeField] int id = -1;
    [SerializeField] int price = 100;

    public static event Action<Building> ServerOnBuildingSpawned;
    public static event Action<Building> ServerOnBuildingDespawned;
    public static event Action<Building> AuthorityOnBuildingSpawned;
    public static event Action<Building> AuthorityOnBuildingDespawned;


    #region Getters
    public Sprite GetIcon()
    {
        return icon;
    }

    public int GetID()
    {
        return id;
    }

    public int GetPrice()
    {
        return price;
    }

    public GameObject GetBuildingPreview()
    {
        return buildingPreview;
    }
    #endregion


    #region Server

    public override void OnStartServer()
    {
        ServerOnBuildingSpawned?.Invoke(this);
    }


    public override void OnStopServer()
    {
        ServerOnBuildingDespawned?.Invoke(this);
    }

    #endregion





    #region Client


    public override void OnStartAuthority()
    {
        AuthorityOnBuildingSpawned?.Invoke(this);
    }

    public override void OnStopClient()
    {
        if (!hasAuthority)
            return;

        AuthorityOnBuildingDespawned?.Invoke(this);
    }

    #endregion

}
