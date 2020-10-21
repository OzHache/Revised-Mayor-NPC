﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public List<InventoryCell> inventoryCells = new List<InventoryCell>();

    private void Start()
    {
       foreach(InventoryCell cell in inventoryCells)
        {
            cell.SetInventorySystem(this);
        }
    }
    public void AddToInventory(InventoryItem item) {
        foreach(InventoryCell cell in inventoryCells)
        {
            //If the item is in the cell and it is reuseable, add it to the count
            if (cell.item == item && item.isConsumeable)
            {
                //If this item exist, add one

                cell.AddOne();
                Debug.Log(item.name + " has been added to " + inventoryCells.IndexOf(cell));
                return;
            }
        }
        //Otherwise see if there is a free space
          
         foreach (InventoryCell cell in inventoryCells)
        {
            if(cell.item == null)
            {
                cell.AddItem(item);
                Debug.Log(item.name + " has been placed in to " + inventoryCells.IndexOf(cell));
                return; 
            }
        }
    }
}
