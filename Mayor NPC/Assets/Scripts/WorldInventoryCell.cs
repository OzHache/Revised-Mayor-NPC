using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WorldInventoryCell : InventoryCell, IEndDragHandler
{
    [SerializeField]WorldInventory inventory;

    public void OnEndDrag(PointerEventData eventData)
    {
        inventory.OnMouseOff();
    }

    public void OnMouseDrag()
    {
        inventory.OnMouseOn();
    }
    private void OnMouseEnter()
    {
        inventory.OnMouseOn();
    }
    private void OnMouseExit()
    {
        inventory.OnMouseOff();
    }
    private void OnMouseOver()
    {
        inventory.OnMouseOn();
        
    }

}
