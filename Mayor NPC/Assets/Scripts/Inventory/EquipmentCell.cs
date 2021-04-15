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
    [SerializeField] private Quest questPartOne;
    [SerializeField] private Quest questPartTwo;

    private new void Start()
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
        // this does not need to do anything
    }
     public void OnButtonClick()
    {
        //this item has not been crafted yet
        if(item == null)
        {
            //Send a Message for the player to start getting the items they will nee
            GameManager.GetGameManager().m_playerController.Say("To make an " + m_lockedItem.name + " you will need "+ m_lockedItem.GetRecipie().inputOne.name+ " and " + m_lockedItem.GetRecipie().inputTwo);
            //send messages to the quest loaded for each quest to load in

            questPartOne.SetDiscovered(true);
            questPartTwo.SetDiscovered(true);
            questPartOne.Reset();
            questPartTwo.Reset();
            QuestManager.GetQuestManager().AddQuest(questPartOne);
            QuestManager.GetQuestManager().AddQuest(questPartTwo);


        }
    }

    internal override void AddItem(InventoryItem newItem, int amount = 1)
    {
        //todo: turn on the image and find where to turn off the item image.
        m_lockImage.color = Vector4.zero;
        base.AddItem(newItem, amount);
    }
} 

