using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public List<InventoryCell> m_inventoryCells = new List<InventoryCell>();
    public List<EquipmentCell> m_equipmentCells = new List<EquipmentCell>();
    private void Start()
    {
        foreach (InventoryCell cell in m_inventoryCells)
        {
            cell.SetInventorySystem(this);
        }
    }
    public void AddToInventory(InventoryItem item, int amount)
    {
        //update the quest manager with this new information
        PlayerActions action;
        action.m_action = Quest.ActionType.Collect;
        action.m_keyWord = item.itemName;
        action.m_number = amount;
        QuestManager.GetQuestManager().UpdateQuests(action);

        foreach (InventoryCell cell in m_inventoryCells)
        {
            bool isEquipment = item is IEquipment;

            if (isEquipment)
            {
                //try and place this item in an inventory
                foreach (EquipmentCell slot in m_equipmentCells)
                {
                    if (slot.m_lockedItem == item)
                    {
                        slot.AddItem(item);
                        return;
                    }
                }
            }

            //If the item is in the cell and it is reuseable, add it to the count
            if (cell.item == item && item.isConsumeable)
            {
                //If this item exist, add one

                cell.Add(amount);
                Debug.Log(item.name + " has been added to " + m_inventoryCells.IndexOf(cell));
                return;
            }
        }
        //Otherwise see if there is a free space

        foreach (InventoryCell cell in m_inventoryCells)
        {
            if (cell.item == null)
            {
                cell.AddItem(item, amount);
                Debug.Log(item.name + " has been placed in to " + m_inventoryCells.IndexOf(cell));
                return;
            }
        }
    }

    internal bool IsSpaceAvailable(InventoryItem itemToCheck)
    {
        foreach (InventoryCell cell in m_inventoryCells)
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
        //Changing this to look through equipment first
        foreach (EquipmentCell slot in m_equipmentCells)
        {
            //continue if the cell is empty
            if (slot.item == null)
            {
                continue;
            }
            //check if this is a tool
            if (slot.item.IsTool())
            {
                //see if this tool is already in the list,
                //if not add it
                tools.Add(slot.item.GetToolType());
            }
        }
        foreach (InventoryCell cell in m_inventoryCells)
        {
            //continue if the cell is empty
            if (cell.item == null)
            {
                continue;
            }
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
