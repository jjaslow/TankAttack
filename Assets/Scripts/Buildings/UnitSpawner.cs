using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{

    [SerializeField] Unit unitPrefab;
    [SerializeField] Transform spawnPoint;
    UnitSelectionHandler unitSelectionHandler = null;

    Health health;
    MyPlayer player;

    [SerializeField] TMP_Text remainingUnitsText;
    [SerializeField] Image unitProgressImage;
    [SerializeField] int maxUnitQueue = 5;
    [SerializeField] float unitSpawnDuration = 5;
    float spawnMoveRange = 5;

    [SyncVar(hook =nameof(ClientHandleQueuedUnitsUpdated))]
    int queuedUnits = 0;
    [SyncVar(hook =nameof(ClientHandleTimerUpdated))]
    float unitTimer;



    #region Server

    [ServerCallback]
    public override void OnStartServer()
    {
        player = connectionToClient.identity.GetComponent<MyPlayer>();

        health = GetComponent<Health>();
        health.ServerOnDie += ServerHandleDie;

        queuedUnits = 0;
        unitTimer = unitSpawnDuration;
    }

    [ServerCallback]
    public override void OnStopServer()
    {
        health.ServerOnDie -= ServerHandleDie;
    }


    [ServerCallback]
    private void Update()
    {

        if (queuedUnits == 0)
            return;

        unitTimer -= Time.deltaTime;

        if (unitTimer <= 0)
        {
            unitTimer = unitSpawnDuration;
            queuedUnits--;
            CmdSpawnUnit();
        }
    }

    [Server]
    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    [Command]
    void CmdQueueUnit()
    {
        int resourcesAvailable = player.GetResources();
        int resourceCost = unitPrefab.GetResourceCost();

        if (resourcesAvailable < resourceCost)
            return;

        if (queuedUnits < maxUnitQueue)
        {
            queuedUnits++;
            player.SetResources(resourcesAvailable - resourceCost);
        }
    }




    //[Command]
    void CmdSpawnUnit()
    {
        GameObject unitInstance = Instantiate(unitPrefab.gameObject, spawnPoint.position, spawnPoint.rotation);
        NetworkServer.Spawn(unitInstance, connectionToClient);

        Vector3 spawnOffset = spawnMoveRange * UnityEngine.Random.onUnitSphere;
        spawnOffset.y = unitInstance.transform.position.y;

        UnitMovement unitMovement = unitInstance.GetComponent<UnitMovement>();
        unitMovement.ServerMove(unitInstance.transform.position + spawnOffset);

        //TargetSelectNewlySpawnedUnit(connectionToClient, unitInstance);
    }



    #endregion







    #region Client

    public override void OnStartAuthority()
    {
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

        CmdQueueUnit();             //CmdSpawnUnit();
    }



    [TargetRpc]
    void TargetSelectNewlySpawnedUnit(NetworkConnection target, GameObject unitObject)
    {
        unitSelectionHandler.SelectNewlySpawnedUnit(unitObject);
    }

    [Client]
    void ClientHandleQueuedUnitsUpdated(int oldUnits, int newUnits)
    {
        remainingUnitsText.text = newUnits.ToString();
    }

    [Client]
    void ClientHandleTimerUpdated(float oldTimer, float newTimer)
    {
        unitProgressImage.fillAmount = newTimer / unitSpawnDuration;
    }


    #endregion

}
