using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class UnitMovement : NetworkBehaviour
{

    [SerializeField]
    NavMeshAgent agent;

    Camera cam;



    #region Server
    [Command]
    private void CmdMove(Vector3 destination)
    {
        //is the point on a navMesh?
        NavMeshHit hit;
        if (!NavMesh.SamplePosition(destination, out hit, 1f, NavMesh.AllAreas))
            return;

        //if so, move player on server (I think that b/c player has a NetworkTransform that is is synced with no need for a SyncVar)
        agent.SetDestination(hit.position);
    }

    #endregion




    #region Client

    //the start method for this client
    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        cam = Camera.main;
    }

    [ClientCallback]
    private void Update()
    {
        //need to make sure this doesnt run from other clients
        if (!hasAuthority)
            return;

        //Get touch/click point
#if UNITY_STANDALONE_WIN
        if (!Mouse.current.rightButton.wasPressedThisFrame)
            return;
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
#else
        if (Input.touchCount == 0)
            return;
        Ray ray = cam.ScreenPointToRay(Input.GetTouch(0).position);
#endif

        //does that point hit the ground (more specifically a collider)?
        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            return;

        //if so, move player (on server)
        CmdMove(hit.point);
    }


    #endregion

}
