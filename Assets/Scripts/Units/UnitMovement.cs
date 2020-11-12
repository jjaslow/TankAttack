using UnityEngine;
using Mirror;
using UnityEngine.AI;

public class UnitMovement : NetworkBehaviour
{

    [SerializeField]
    NavMeshAgent agent;

    [SerializeField]
    float chaseDistance = 3f;

    Targeter targeter = null;



    #region Server

    [ServerCallback]
    private void Start()
    {
        targeter = GetComponent<Targeter>();
    }

    [ServerCallback]
    private void Update()
    {
        Targetable currentTarget = targeter.GetTarget();

        if (currentTarget != null)
        {
            if (Vector3.Distance(transform.position, currentTarget.transform.position) < chaseDistance)
            {
                agent.ResetPath();
                return;
            }

            agent.SetDestination(currentTarget.transform.position);
            return;
        }

        if (!agent.hasPath)
            return;
        if (agent.remainingDistance > agent.stoppingDistance)
            return;

        agent.ResetPath();
    }


    [Command]
    public void CmdMove(Vector3 destination)
    {
        targeter.ClearTarget();


        //is the point on a navMesh?
        NavMeshHit hit;
        if (!NavMesh.SamplePosition(destination, out hit, 1f, NavMesh.AllAreas))
            return;

        //if so, move player on server (I think that b/c player has a NetworkTransform that is is synced with no need for a SyncVar)
        agent.SetDestination(hit.position);
    }

 
    #endregion






}
