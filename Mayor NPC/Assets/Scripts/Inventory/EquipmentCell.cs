using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public enum EquipmentTypes
{
    Weapon, Tool, Head, Body
}
public class EquipmentCell : InventoryCell
{
    [SerializeField] private EquipmentTypes equipmentType;
    public override void OnDrop(PointerEventData eventData)
    {
        InventoryItem dropItem = MouseInventory.GetMouseInvUI().hasItem;
        //see if the mouse inventory has an item
        if (dropItem != null)
        {
            //check if this item is of the right type
            if(dropItem is IEquipment)
            {
                if ((dropItem as IEquipment).GetEquipmentTypes() == equipmentType)
                    base.OnDrop(eventData);
            }
        }
    }
            
} 

