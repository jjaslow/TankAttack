using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitCommandGiver : MonoBehaviour
{
    [SerializeField]
    UnitSelectionHandler unitSelectionHandler = null;

    Camera mainCamera;
    [SerializeField] LayerMask layermask = new LayerMask();


    private void Start()
    {
        mainCamera = Camera.main;
        GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
    }

    private void OnDisable()
    {
        GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
    }

    private void Update()
    {
        //Get touch/click point
#if UNITY_STANDALONE_WIN
        if (!Mouse.current.rightButton.wasPressedThisFrame)
            return;

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
#else
        if (Input.touchCount == 0)
            return;
        Ray ray = mainCamera.ScreenPointToRay(Input.GetTouch(0).position);
#endif

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layermask))
            return;

        Debug.Log("I targeted: " + hit.collider.name);

        //we definitely hit something...
        //if we click on a Targetable (not our own) then we target it (chase it), else we just go to that place.
        if(hit.collider.TryGetComponent<Targetable>(out Targetable target))
        {
            if(target.hasAuthority)
            {
                TryMove(hit.point);
                return;
            }
            TryTarget(target);
            return;
        }

        TryMove(hit.point);
    }



    private void TryMove(Vector3 point)
    {
        foreach(Unit unit in unitSelectionHandler.GetSelectedUnits())
        {
            unit.GetComponent<UnitMovement>().CmdMove(point);
        }
    }

    private void TryTarget(Targetable target)
    {
        foreach (Unit unit in unitSelectionHandler.GetSelectedUnits())
        {
            unit.GetComponent<Targeter>().CmdSetTarget(target.gameObject);
        }
    }


    void ClientHandleGameOver(string winnerName)
    {
        enabled = false;
    }


}
