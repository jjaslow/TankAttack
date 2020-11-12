using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{

    [SerializeField] GameObject unitPrefab;
    [SerializeField] Transform spawnPoint;
    UnitSelectionHandler unitSelectionHandler = null;

    Health health;


    #region Server

    [ServerCallback]
    public override void OnStartServer()
    {
        health = GetComponent<Health>();
        health.ServerOnDie += ServerHandleDie;
    }

    [ServerCallback]
    public override void OnStopServer()
    {
        health.ServerOnDie -= ServerHandleDie;
    }


    [Server]
    private void ServerHandleDie()
    {
        //NetworkServer.Destroy(gameObject);
    }

    [Command]
    void CmdSpawnUnit()
    {
        GameObject unitInstance = Instantiate(unitPrefab, spawnPoint.position, spawnPoint.rotation);
        NetworkServer.Spawn(unitInstance, connectionToClient);

        TargetSelectNewlySpawnedUnit(connectionToClient, unitInstance);
    }



    #endregion







    #region Client

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        unitSelectionHandler = GameObject.Find("UnitHandlers").GetComponent<UnitSelectionHandler>();
    }

    //detects clicks on items. Needs EventHandler game object
    //also need to add Physics Raycaster component TO CAMERA (since this is detecting clicks on game objects).
    //(if it was detecting clicks on UI elements then we have the Graphic Raycaster which is on the canvas)
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (!hasAuthority)
            return;

        CmdSpawnUnit();
    }

    [TargetRpc]
    void TargetSelectNewlySpawnedUnit(NetworkConnection target, GameObject unitObject)
    {
        unitSelectionHandler.SelectNewlySpawnedUnit(unitObject);
    }


    #endregion

}
