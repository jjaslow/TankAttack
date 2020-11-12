using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targeter : NetworkBehaviour
{
    Targetable target;

    public Targetable GetTarget()
    {
        return target;
    }

    #region Server

    [Command]
    public void CmdSetTarget(GameObject targetGameObject)
    {
        //already have checked to see if its targetable before we even call this method
        //if (!targetGameObject.TryGetComponent<Targetable>(out Targetable target))
        //    return;
        //this.target = target;

        this.target = targetGameObject.GetComponent<Targetable>();
    }


    [Server]
    public void ClearTarget()
    {
        target = null;
    }


    #endregion

}
