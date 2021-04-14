using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum EquipmentTypes
{
    Weapon, Tool, Head, Body
}
public class EquipmentCell : InventoryCell
{
    [SerializeField]private Image m_lockImage;
    private void Start()
    {
        if (lockedInventory)
        {
            m_lockImage.sprite = m_lockedItem.art;
            //lower the alpha
            var color = m_lockImage.color;
            color.a = 0.25f;
            m_lockImage.color = color;
        }
    }
    public override void OnDrop(PointerEventData eventData)
    {
        //this should not succeed if there is a drop event that includes this. 
       /* InventoryItem dropItem = MouseInventory.GetMouseInvUI().GetItem();
        //see if the mouse inventory has an item
        if (dropItem != null)
        {
            //check if this item is of the right type
            if(dropItem is IEquipment)
            {
                if ((dropItem as IEquipment).GetEquipmentTypes() == equipmentType)
                    base.OnDrop(eventData);
            }
        }*/
    }
            
} 

