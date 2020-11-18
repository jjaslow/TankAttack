using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : NetworkBehaviour
{

    [SerializeField] int maxHealth = 100;

    [SerializeField]
    [SyncVar(hook=nameof(HandleHealthUpdated))]
    int currentHealth;



    //passing current and max health
    public event Action<int, int> ClientOnHealthUpdated;
    public event Action ServerOnDie;


    #region Server

    public override void OnStartServer()
    {
        currentHealth = maxHealth;
        UnitBase.ServerOnPlayerDie += ServerHandlePlayerDie;
    }

    public override void OnStopServer()
    {
        UnitBase.ServerOnPlayerDie -= ServerHandlePlayerDie;
    }

    [Server]
    public void DealDamage(int damageAmount)
    {
        if (currentHealth <= 0)
            return;

        currentHealth -= damageAmount;
        if (currentHealth < 0)
            currentHealth = 0;

        if (currentHealth==0)
        {
            //dead
            Debug.Log(this.name + " Died");
            ServerOnDie?.Invoke();
        }
    }

    [Server]
    void ServerHandlePlayerDie(int connectionID)
    {
        if (connectionToClient.connectionId != connectionID)
            return;

        //HERE
        //ServerOnDie?.Invoke();
        DealDamage(currentHealth);
    }


    #endregion







    #region Client

    private void HandleHealthUpdated(int oldHealth, int newHealth)
    {
        ClientOnHealthUpdated?.Invoke(newHealth, maxHealth);
    }


    #endregion





}
