using UnityEngine;
using Mirror;
using UnityEngine.AI;

public class UnitMovement : NetworkBehaviour
{

    [SerializeField]
    NavMeshAgent agent;



    #region Server
    [Command]
    public void CmdMove(Vector3 destination)
    {
        //is the point on a navMesh?
        NavMeshHit hit;
        if (!NavMesh.SamplePosition(destination, out hit, 1f, NavMesh.AllAreas))
            return;

        //if so, move player on server (I think that b/c player has a NetworkTransform that is is synced with no need for a SyncVar)
        agent.SetDestination(hit.position);
    }

    #endregion




   

}
