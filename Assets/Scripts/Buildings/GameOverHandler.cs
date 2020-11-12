using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverHandler : NetworkBehaviour
{
    [SerializeField]
    List<UnitBase> unitBases = new List<UnitBase>();

    #region Server

    public override void OnStartServer()
    {
        base.OnStartServer();

        UnitBase.ServerOnBaseSpawn += ServerHandleBaseSpawned;
        UnitBase.ServerOnBaseDespawn += ServerHandleBaseDespawned;
    }

    public override void OnStopServer()
    {
        base.OnStopServer();

        UnitBase.ServerOnBaseSpawn -= ServerHandleBaseSpawned;
        UnitBase.ServerOnBaseDespawn -= ServerHandleBaseDespawned;
    }


    [Server]
    void ServerHandleBaseSpawned(UnitBase unitBase)
    {
        unitBases.Add(unitBase);
    }

    [Server]
    void ServerHandleBaseDespawned(UnitBase unitBase)
    {
        unitBases.Remove(unitBase);

        if (unitBases.Count != 1)
            return;

        Debug.Log("Game Over");
    }

    #endregion




    #region Client



    #endregion



}
