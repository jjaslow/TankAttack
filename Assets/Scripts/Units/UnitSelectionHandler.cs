using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSelectionHandler : MonoBehaviour
{
    Camera mainCamera;
    [SerializeField] LayerMask layermask = new LayerMask();

    public List<Unit> selectedUnits { get; } = new List<Unit>();



    private void Start()
    {
        mainCamera = Camera.main;
    }




    private void Update()
    {
        if(Mouse.current.leftButton.wasPressedThisFrame)
        {
            //Start selection area
            Deselect();
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            ClearSelectionArea();
        }
    }

    private void Deselect()
    {
        foreach (Unit selectedUnit in selectedUnits)
            selectedUnit.Deselect();

        selectedUnits.Clear();
    }

    private void ClearSelectionArea()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layermask))
            return;

        if (!hit.collider.TryGetComponent<Unit>(out Unit unit))
            return;

        if (!unit.hasAuthority)
            return;

        selectedUnits.Add(unit);

        foreach (Unit selectedUnit in selectedUnits)
            selectedUnit.Select();
    }

    public void SelectNewlySpawnedUnit(GameObject unitObject)
    {
        //Deselect();

        Unit unit = unitObject.GetComponent<Unit>();
        selectedUnits.Add(unit);
        unit.Select();
    }
}
