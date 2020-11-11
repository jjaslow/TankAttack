using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSelectionHandler : MonoBehaviour
{
    Camera mainCamera;
    [SerializeField] LayerMask layermask = new LayerMask();

    [SerializeField] RectTransform dragBox = null;
    MyPlayer player = null;
    Vector2 dragStartPosition;

    public List<Unit> selectedUnits { get; } = new List<Unit>();



    private void Start()
    {
        mainCamera = Camera.main;
        //player = NetworkClient.connection.identity.GetComponent<MyPlayer>();
    }




    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            StartSelectionArea();
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            ClearSelectionArea();
        }
        else if (Mouse.current.leftButton.isPressed)
        {
            UpdateSelectionArea();
        }
    }


    private void StartSelectionArea()
    {
        Deselect();

        dragBox.gameObject.SetActive(true);
        dragStartPosition = Mouse.current.position.ReadValue();
        UpdateSelectionArea();
    }


    private void UpdateSelectionArea()
    {
        Vector2 dragCurrentPosition = Mouse.current.position.ReadValue();

        float width = dragCurrentPosition.x - dragStartPosition.x;
        float height = dragCurrentPosition.y - dragStartPosition.y;

        dragBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
        dragBox.anchoredPosition = new Vector2(dragStartPosition.x + width / 2, dragStartPosition.y + height / 2);
    }

    private void ClearSelectionArea()
    {
        dragBox.gameObject.SetActive(false);

        if(dragBox.sizeDelta.magnitude == 0)
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

            return;
        }

        Vector2 min = dragBox.anchoredPosition - (dragBox.sizeDelta / 2);
        Vector2 max = dragBox.anchoredPosition + (dragBox.sizeDelta / 2);

        //temp requirement to get player, since its still null at Start
        if (player == null)
            player = NetworkClient.connection.identity.GetComponent<MyPlayer>();

        foreach (Unit unit in player.GetMyUnits())
        {
            Vector3 unitScreenPosition = mainCamera.WorldToScreenPoint(unit.transform.position);

            if(unitScreenPosition.x >= min.x 
                && unitScreenPosition.x <= max.x 
                && unitScreenPosition.y >= min.y 
                && unitScreenPosition.y <= max.y)
            {
                selectedUnits.Add(unit);
                unit.Select();
            }
        }
    }


    private void Deselect()
    {
        foreach (Unit selectedUnit in selectedUnits)
            selectedUnit.Deselect();

        selectedUnits.Clear();
    }



    public void SelectNewlySpawnedUnit(GameObject unitObject)
    {
        //Deselect();

        Unit unit = unitObject.GetComponent<Unit>();
        selectedUnits.Add(unit);
        unit.Select();
    }
}
