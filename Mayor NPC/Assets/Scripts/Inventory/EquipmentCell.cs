using System;
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
    [SerializeField] private Image m_lockImage;
    private List<Quest> quests;
    private Color m_startingColor;

    private new void Start()
    {
        if (lockedInventory)
        {
            m_lockImage.sprite = m_lockedItem.art;
            //lower the alpha
            var color = m_lockImage.color;
            color.a = 0.25f;
            m_lockImage.color = color;
            //disable the item icon
            m_startingColor = color;
            image.color = Vector4.zero;
        }

        BuildQuest();
    }

    private void BuildQuest()
    {
        //check the recipie for quest
        if (m_lockedItem != null)
        {
            quests = m_lockedItem.GetQuest();
        }

    }

    public override void OnDrop(PointerEventData eventData)
    {
        // this does not need to do anything
    }
     public void OnButtonClick()
    {
        //this item has not been crafted yet
        if(item == null)
        {
            string recipie = "To make an " + m_lockedItem.name + " you will need ";
            var ingredients = m_lockedItem.GetRecipie().Ingredients();
            if (ingredients != null)
                foreach (var item in ingredients)
                {
                    recipie += item;
                    if (ingredients.IndexOf(item) != ingredients.Count - 1) {
                        recipie += " and ";
                    }
                    else
                    {
                        recipie += ".";
                    }
                }

            //Send a Message for the player to start getting the items they will nee
            GameManager.GetGameManager().m_playerController.Say(recipie);
            //send messages to the quest loaded for each quest to load in
            foreach(Quest quest in quests)
            {
                QuestManager.GetQuestManager().AddQuest(quest);
            }
        }
    }

    internal override void AddItem(InventoryItem newItem, int amount = 1)
    {
        //todo: turn on the image and find where to turn off the item image.
        m_lockImage.color = Vector4.zero;
        image.color = m_startingColor;
        base.AddItem(newItem, amount);
    }
} 

