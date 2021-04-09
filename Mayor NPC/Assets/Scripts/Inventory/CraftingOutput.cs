﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class CraftingOutput : InventoryCell
{
    //refernece to the Crafting Station
    [SerializeField] private CraftingStation craftingStation;

    override internal void RemoveOne()
    {
        numberOfItems--;
      
        UpdateUI();
        craftingStation.Activated();
        //Whenever an Item is crafted send a message to the questing system
        PlayerActions action = new PlayerActions();
        action.m_action = Quest.Action.Craft;
        action.m_keyWord = item.name;
        action.m_number = 1;
        QuestManager.GetQuestManager().UpdateQuests(action);  
        if (numberOfItems == 0 || (item.isReuseable && durability <= 0))
        {
            Clear();
        }

    }
}
