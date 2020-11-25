using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamColorSetter : NetworkBehaviour
{
    [SerializeField] Renderer[] rends = new Renderer[0];

    [SyncVar(hook = nameof(ClientHandleColorUpdated))] Color myColor = new Color();

    #region Server
    public override void OnStartServer()
    {
        MyPlayer player = connectionToClient.identity.GetComponent<MyPlayer>();

        myColor = player.GetMyColor();

    }

    #endregion




    #region Client

    void ClientHandleColorUpdated(Color oldColor, Color newColor)
    {
        foreach (Renderer renderer in rends)
        {
            renderer.material.color = newColor;
        }
    }

    #endregion


}
