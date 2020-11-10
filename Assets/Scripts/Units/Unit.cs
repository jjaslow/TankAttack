using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Unit : NetworkBehaviour
{

    [SerializeField] UnityEvent onSelected = null;

    [SerializeField] UnityEvent onDeselected = null;



    #region Client
    //Server doesnt need to know which is selected. 
    //Locally we will send the appropriate command from the selected object.
    [Client]
    public void Select()
    {
        if (!hasAuthority)
            return;

        onSelected?.Invoke();

    }

    [Client]
    public void Deselect()
    {
        if (!hasAuthority)
            return;

        onDeselected?.Invoke();

    }



    #endregion

}
