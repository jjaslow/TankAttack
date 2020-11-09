using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{

    [SerializeField] GameObject unitPrefab;
    [SerializeField] Transform spawnPoint;



    #region Server

    [Command]
    void CmdSpawnUnit()
    {
        GameObject unitInstance = Instantiate(unitPrefab, spawnPoint.position, spawnPoint.rotation);
        NetworkServer.Spawn(unitInstance, connectionToClient);
    }


    #endregion







    #region Client

    //detects clicks on items. Needs EventHandler game object
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (!hasAuthority)
            return;

        CmdSpawnUnit();
    }


    #endregion

}
