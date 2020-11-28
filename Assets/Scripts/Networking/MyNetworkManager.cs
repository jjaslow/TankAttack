using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MyNetworkManager : NetworkManager
{
    [Space]
    [SerializeField] GameObject basePrefab;
    [SerializeField] GameOverHandler gameOverHandlerPrefab;

    public List<MyPlayer> Players { get; } = new List<MyPlayer>();
    bool isGameInProgress = false;

    public static event Action ClientOnConnected;
    public static event Action ClientOnDisconnected;




    #region Server

    public override void OnServerConnect(NetworkConnection conn)
    {
        if (!isGameInProgress) return;

        conn.Disconnect();
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        MyPlayer player = conn.identity.GetComponent<MyPlayer>();
        Players.Remove(player);

        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer()
    {
        Players.Clear();
        isGameInProgress = false;
    }

    public void StartGame()
    {
        if (Players.Count < 2) return;

        isGameInProgress = true;

        ServerChangeScene("Map_01");
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        MyPlayer player = conn.identity.GetComponent<MyPlayer>();

        Players.Add(player);

        Color newColor = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
        player.SetMyColor(newColor);

        player.SetPartyOwner(Players.Count == 1);
        player.SetDisplayName("Player " + Players.Count);
    }


    public override void OnServerSceneChanged(string sceneName)
    {
        if (!SceneManager.GetActiveScene().name.Contains("Map"))
        {
            return;
        }

        GameOverHandler gameOverHandlerInstance = Instantiate(gameOverHandlerPrefab);
        NetworkServer.Spawn(gameOverHandlerInstance.gameObject);

        foreach(MyPlayer player in Players)
        {
            GameObject baseInstance = Instantiate(basePrefab, GetStartPosition().position, Quaternion.identity);
            NetworkServer.Spawn(baseInstance, player.connectionToClient);
        }
    }

    #endregion





    #region Client

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        ClientOnConnected?.Invoke();
    }
    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        ClientOnDisconnected?.Invoke();
    }

    public override void OnStopClient()
    {
        Players.Clear();
    }

    #endregion
}
