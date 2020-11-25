using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MyNetworkManager : NetworkManager
{
    [Space]
    [SerializeField] GameObject basePrefab;
    [SerializeField] GameOverHandler gameOverHandlerPrefab;


    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        MyPlayer player = conn.identity.GetComponent<MyPlayer>();
        Color newColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        player.SetMyColor(newColor);

        GameObject baseInstance = Instantiate(basePrefab, conn.identity.transform.position, conn.identity.transform.rotation);
        NetworkServer.Spawn(baseInstance, conn);
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
