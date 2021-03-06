﻿using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BuildingButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    [SerializeField] Building building = null;
    [SerializeField] Image iconImage = null;
    [SerializeField] TMP_Text priceText;

    GameObject buildingPreviewInstance;
    Renderer buildingRendererInstance;
    static bool isSelecting = false;
    Material mat;
    Color defaultColor;

    [SerializeField] LayerMask floorMask = new LayerMask();
    Camera mainCamera;
    MyPlayer myPlayer;



    private void Start()
    {
        mainCamera = Camera.main;
        
        iconImage.sprite = building.GetIcon();
        priceText.text = building.GetPrice().ToString();

        myPlayer = NetworkClient.connection.identity.GetComponent<MyPlayer>();
    }

    public static bool IsSelectingBuilding()
    {
        return isSelecting;
    }

    private void Update()
    {
        if (buildingPreviewInstance != null)
            UpdateBuildingPreview();
    }



    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        //check funds
        if (myPlayer.GetResources() < building.GetPrice())
            return;

        isSelecting = true;

        buildingPreviewInstance = Instantiate(building.GetBuildingPreview());
        buildingPreviewInstance.SetActive(false);

        buildingRendererInstance = buildingPreviewInstance.GetComponentInChildren<Renderer>();

        mat = buildingRendererInstance.GetComponentInChildren<Renderer>().material;
        defaultColor = mat.color;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (buildingPreviewInstance == null)
            return;

        isSelecting = false;
        
        //LOCATION TEST

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if(Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, floorMask))
        {
            //place building
            myPlayer.CmdTryPlaceBuilding(building.GetID(), hit.point);
        }

        Destroy(buildingPreviewInstance);
    }

    void UpdateBuildingPreview()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, floorMask))
        {
            buildingPreviewInstance.transform.position = hit.point;

            if (!myPlayer.CanPlaceBuildingHere(building, hit.point))
            {
                mat.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, .25f);
            }
            else
                mat.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 1f);

            if (!buildingPreviewInstance.activeSelf)
                buildingPreviewInstance.SetActive(true);
        }

        
    }



}
