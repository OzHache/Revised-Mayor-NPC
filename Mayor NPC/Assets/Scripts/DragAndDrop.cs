using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class DragAndDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 dropPosition;
    //the cell that this item is coming from
    private InventoryCell cell;

    private void Start()
    {
        cell = gameObject.GetComponent<InventoryCell>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //pass the sprite of this gameobject to the mouseUI
        MouseInventory.GetMouseInvUI().ActivateIcon(gameObject.GetComponentInChildren<Image>().sprite);
    }

    public void OnDrag(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        dropPosition = MouseInventory.GetMouseInvUI().DeactivateIcon();
        //todo: If this is in the world, Take the item out of the inventory and place it in the world
        

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // new System.NotImplementedException();
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }
}
