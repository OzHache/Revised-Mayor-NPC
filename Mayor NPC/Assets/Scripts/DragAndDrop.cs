using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class DragAndDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler
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

        //Manage mouse over UI

    private void OnMouseEnter()
    {
        MouseUI.GetMouseUI().MouseOver();
        MouseInventory.GetMouseInvUI().SetActiveCell(cell);
    }
    private void OnMouseExit()
    {
        MouseUI.GetMouseUI().MouseExit();
        MouseInventory.GetMouseInvUI().SetActiveCell();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //pass the sprite of this gameobject to the mouseUI
        MouseInventory.GetMouseInvUI().PickUp(cell);
        
    }

    public void OnDrag(PointerEventData eventData)
    {
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
