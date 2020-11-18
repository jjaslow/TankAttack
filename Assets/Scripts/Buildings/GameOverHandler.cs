using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverHandler : NetworkBehaviour
{
    List<UnitBase> unitBases;

    public static event Action ServerOnGameOver;
    public static event Action<string> ClientOnGameOver;

    #region Server

    public override void OnStartServer()
    {
        unitBases = new List<UnitBase>();

        UnitBase.ServerOnBaseSpawn += ServerHandleBaseSpawned;
        UnitBase.ServerOnBaseDespawn += ServerHandleBaseDespawned;
    }

    public override void OnStopServer()
    {
        Debug.Log("GameOverHandler.cs OnStopServer called");

        unitBases.Clear();

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
        //if (this == null) { return; }

        unitBases.Remove(unitBase);

        if (unitBases.Count != 1)
            return;

        int playerID = unitBases[0].connectionToClient.connectionId;
        RpcGameOver("Player " + playerID);

        ServerOnGameOver?.Invoke();
    }

    #endregion




    #region Client


    [ClientRpc]
    void RpcGameOver(string winner)
    {
        ClientOnGameOver?.Invoke(winner);
    }



    #endregion



}
