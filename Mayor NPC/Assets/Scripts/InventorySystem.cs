using System;
using System.Collections;
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
    public void AddToInventory(InventoryItem item, int amount) {
        foreach(InventoryCell cell in inventoryCells)
        {
            //If the item is in the cell and it is reuseable, add it to the count
            if (cell.item == item && item.isConsumeable)
            {
                //If this item exist, add one

                cell.Add(amount);
                Debug.Log(item.name + " has been added to " + inventoryCells.IndexOf(cell));
                return;
            }
        }
        //Otherwise see if there is a free space
          
         foreach (InventoryCell cell in inventoryCells)
        {
            if(cell.item == null)
            {
                cell.AddItem(item, amount);
                Debug.Log(item.name + " has been placed in to " + inventoryCells.IndexOf(cell));
                return; 
            }
        }
    }

    internal bool IsSpaceAvailable(InventoryItem itemToCheck)
    {
        foreach (InventoryCell cell in inventoryCells)
        {
            if (cell.item == null || cell.item == itemToCheck)
            {
                return true;
            }
        }
        //if there is no empty spot and there is no matching items already in inventory
        Debug.Log("There are no empty slots and no matching item in Player Inventory for" + itemToCheck.name);
        return false;
    }

    public List<ToolType> GetTools()
    {
        List<ToolType> tools = new List<ToolType>();
        foreach(InventoryCell cell in inventoryCells)
        {
            //continue if the cell is empty
            if (cell.item == null)
                continue;
            //check if this is a tool
            if (cell.item.IsTool())
            {
                //see if this tool is already in the list,
                //if not add it
                tools.Add(cell.item.GetToolType());
            }
        }
        //send back the list of tools
        return tools;
    }
}
