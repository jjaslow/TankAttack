using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourcesDisplay : MonoBehaviour
{
    [SerializeField] TMP_Text resourcesText;
    MyPlayer player;

    private void Start()
    {
        player = NetworkClient.connection.identity.GetComponent<MyPlayer>();
        ClientHandleResourcesUpdated(player.GetResources());

        player.ClientOnResourcesUpdated += ClientHandleResourcesUpdated;
    }


    private void OnDisable()
    {
        player.ClientOnResourcesUpdated -= ClientHandleResourcesUpdated;
    }

    private void ClientHandleResourcesUpdated(int resources)
    {
        resourcesText.text = "Resources: " + resources.ToString();
    }
}
