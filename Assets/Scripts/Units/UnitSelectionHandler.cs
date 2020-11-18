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

    [SerializeField]
    List<Unit> selectedUnits = new List<Unit>();

    public List<Unit> GetSelectedUnits()
    {
        return selectedUnits;
    }

    private void Start()
    {
        mainCamera = Camera.main;

        //temp move to later...
        //player = NetworkClient.connection.identity.GetComponent<MyPlayer>();
    }

    private void OnEnable()
    {
        GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
        Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;
    }

    private void OnDisable()
    {
        GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
        Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
    }


    private void AuthorityHandleUnitDespawned(Unit unit)
    {
        if (selectedUnits.Contains(unit))
            selectedUnits.Remove(unit);
    }




    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            StartSelectionArea();
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            FinalizeSelection();
        }
        else if (Mouse.current.leftButton.isPressed)
        {
            UpdateSelectionArea();
        }
    }


    private void StartSelectionArea()
    {
        if(!Keyboard.current.shiftKey.IsPressed())
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

    private void FinalizeSelection()
    {
        dragBox.gameObject.SetActive(false);

        //we just clicked on an object, not dragged a whole box. so just select that one item.
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

        //else we dragged a box selection...
        Vector2 min = dragBox.anchoredPosition - (dragBox.sizeDelta / 2);
        Vector2 max = dragBox.anchoredPosition + (dragBox.sizeDelta / 2);

        //temp requirement to get player, since its still null at Start
        if (player == null)
            player = NetworkClient.connection.identity.GetComponent<MyPlayer>();

        foreach (Unit unit in player.GetMyUnits())
        {
            if (selectedUnits.Contains(unit))
                continue;

            //get each of our units screen location. If it is in the box then select it.
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



    private void ClientHandleGameOver(string winnerName)
    {
        enabled = false;
    }


    public void SelectNewlySpawnedUnit(GameObject unitObject)
    {
        //Deselect();

        Unit unit = unitObject.GetComponent<Unit>();
        selectedUnits.Add(unit);
        unit.Select();
    }


    private void Deselect()
    {
        foreach (Unit selectedUnit in selectedUnits)
            selectedUnit.Deselect();

        selectedUnits.Clear();
    }
}
