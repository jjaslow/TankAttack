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

        TryMove(hit.point);
    }

    private void TryMove(Vector3 point)
    {
        foreach(Unit unit in unitSelectionHandler.selectedUnits)
        {
            unit.GetComponent<UnitMovement>().CmdMove(point);
        }
    }
}
