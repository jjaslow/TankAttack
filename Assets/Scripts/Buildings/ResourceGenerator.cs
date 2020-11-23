using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceGenerator : NetworkBehaviour
{

    [SerializeField] Health health;
    [SerializeField] int resourcesPerInterval = 10;
    [SerializeField] float interval = 2f;

    float timer;
    MyPlayer player;



    #region Server

    public override void OnStartServer()
    {
        timer = interval;
        player = connectionToClient.identity.GetComponent<MyPlayer>();

        health.ServerOnDie += ServerHandleDie;
        GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
    }

    public override void OnStopServer()
    {
        health.ServerOnDie -= ServerHandleDie;
        GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
    }

    [Server]
    private void Update()
    {
        timer -= Time.deltaTime;

        if(timer<=0)
        {
            timer += interval;
            player.SetResources(player.GetResources() + resourcesPerInterval);
        }
    }

    void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    void ServerHandleGameOver()
    {
        enabled = false;
    }

    #endregion



}
