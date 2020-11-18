using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class GameOverDisplay : MonoBehaviour
{
    [SerializeField]
    GameObject winnerTextGameobject;
    [SerializeField]
    TMP_Text winnerNameText;


    void OnEnable()
    {
        GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
        winnerTextGameobject.SetActive(false);
    }

    private void OnDisable()
    {
        GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
    }


    void ClientHandleGameOver(string winner)
    {
        winnerTextGameobject.SetActive(true);
        winnerNameText.text = winner + " Has Won!";
    }

    public void LeaveGame()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            //WE are hosting and want to STOP HOSTING
            Debug.Log("GOD stop hosting");
            //NetworkManager.singleton.StopHost();
            NetworkManager.singleton.StopServer();
            NetworkManager.singleton.StopClient();
        }
        else
        {
            //STOP CLIENT
            NetworkManager.singleton.StopClient();
        }

    }

}
