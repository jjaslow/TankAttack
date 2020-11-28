using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MiniMap : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField] RectTransform miniMapRect;
    //[SerializeField] Camera miniMapCam;
    [SerializeField] Transform playerCameraTransform;
    [SerializeField] float mapScale = 20f;
    [SerializeField] float offset = -6;

    private void Update()
    {
        if (playerCameraTransform == null)
            if(NetworkClient.connection.identity!=null)
                playerCameraTransform = NetworkClient.connection.identity.GetComponent<MyPlayer>().GetCameraTransform();
    }

    void MoveCamera()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(miniMapRect, mousePos, null, out Vector2 localPoint))
            return;

        Vector2 lerp = new Vector2(
            (localPoint.x - miniMapRect.rect.x) / miniMapRect.rect.width, 
            (localPoint.y - miniMapRect.rect.y) / miniMapRect.rect.height);

        Vector3 newCameraPos = new Vector3(
            Mathf.Lerp(-mapScale, mapScale, lerp.x),
            playerCameraTransform.position.y,
            Mathf.Lerp(-mapScale, mapScale, lerp.y)
            );

        if (newCameraPos.z < 0)
            newCameraPos.z *= 1.25f;

        playerCameraTransform.position = newCameraPos + new Vector3(0, 0, offset);
    }




    public void OnPointerDown(PointerEventData eventData)
    {
        MoveCamera();
    }

    public void OnDrag(PointerEventData eventData)
    {
        MoveCamera();
    }
}
