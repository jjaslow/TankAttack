using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MyNetworkManager : NetworkManager
{
    [Space]
    [SerializeField] GameObject unitSpawnerPrefab;
    [SerializeField] GameOverHandler gameOverHandlerPrefab;


    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        GameObject unitSpawnerInstance = Instantiate(unitSpawnerPrefab, conn.identity.transform.position, conn.identity.transform.rotation);
        NetworkServer.Spawn(unitSpawnerInstance, conn);

    }



    public override void OnServerSceneChanged(string sceneName)
    {


        if (!SceneManager.GetActiveScene().name.Contains("Map"))
        {
            return;
        }

        GameOverHandler gameOverHandlerInstance = Instantiate(gameOverHandlerPrefab);
        NetworkServer.Spawn(gameOverHandlerInstance.gameObject);
    }




}
