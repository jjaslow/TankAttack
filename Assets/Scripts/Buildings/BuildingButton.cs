using Mirror;
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

    [SerializeField] LayerMask floorMask = new LayerMask();
    Camera mainCamera;
    MyPlayer myPlayer;



    private void Start()
    {
        mainCamera = Camera.main;
        
        iconImage.sprite = building.GetIcon();
        priceText.text = building.GetPrice().ToString();


    }

    public static bool IsSelectingBuilding()
    {
        return isSelecting;
    }

    private void Update()
    {
        //temp requirement to get player, since its still null at Start
        if (myPlayer == null)
            myPlayer = NetworkClient.connection.identity.GetComponent<MyPlayer>();

        if (buildingPreviewInstance != null)
            UpdateBuildingPreview();
    }



    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        isSelecting = true;

        buildingPreviewInstance = Instantiate(building.GetBuildingPreview());
        buildingPreviewInstance.SetActive(false);

        buildingRendererInstance = buildingPreviewInstance.GetComponentInChildren<Renderer>();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (buildingPreviewInstance == null)
            return;

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if(Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, floorMask))
        {
            //place building
            myPlayer.CmdTryPlaceBuilding(building.GetID(), hit.point);
        }

        Destroy(buildingPreviewInstance);

        isSelecting = false;
    }

    void UpdateBuildingPreview()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, floorMask))
        {
            buildingPreviewInstance.transform.position = hit.point;

            if(!buildingPreviewInstance.activeSelf)
                buildingPreviewInstance.SetActive(true);
        }

        
    }

}
