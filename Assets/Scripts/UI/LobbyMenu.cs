using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] GameObject lobbyUI;
    [SerializeField] Button startGameButton;
    [SerializeField] TMP_Text[] playerNameTexts = new TMP_Text[4];

    private void OnEnable()
    {
        MyNetworkManager.ClientOnConnected += HandleClientConnected;
        MyPlayer.AuthorityOnPartyOwnerStateUpdated += AuthorityHandlePartyOwnerStateUpdated;
        MyPlayer.ClientOnInfoUpdated += ClientHandleInfoUpdated;
    }

    private void OnDisable()
    {
        MyNetworkManager.ClientOnConnected -= HandleClientConnected;
        MyPlayer.AuthorityOnPartyOwnerStateUpdated -= AuthorityHandlePartyOwnerStateUpdated;
        MyPlayer.ClientOnInfoUpdated -= ClientHandleInfoUpdated;
    }


    private void HandleClientConnected()
    {
        lobbyUI.SetActive(true);
    }

    private void ClientHandleInfoUpdated()
    {
        List<MyPlayer> players = ((MyNetworkManager)NetworkManager.singleton).Players;

        for(int i=0; i<players.Count; i++)
        {
            playerNameTexts[i].text = players[i].GetDisplayName();
        }
        for (int i = players.Count; i < playerNameTexts.Length; i++)
        {
            playerNameTexts[i].text = "Waiting for Player...";
        }

        startGameButton.interactable = players.Count >= 2;
    }

    public void LeaveLobby()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
            NetworkManager.singleton.StopHost();
        else
        {
            NetworkManager.singleton.StopClient();
            SceneManager.LoadScene(0);
        }
    }

    public void StartGame()
    {
        NetworkClient.connection.identity.GetComponent<MyPlayer>().CmdStartGame();
    }

    private void AuthorityHandlePartyOwnerStateUpdated(bool state)
    {
        startGameButton.gameObject.SetActive(state);
    }
}
