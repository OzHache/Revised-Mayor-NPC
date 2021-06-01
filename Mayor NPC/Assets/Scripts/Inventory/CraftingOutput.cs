using UnityEngine;

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
        action.m_action = Quest.ActionType.Craft;
        action.m_keyWord = item.name;
        action.m_number = 1;
        QuestManager.GetQuestManager().UpdateQuests(action);
        if (numberOfItems == 0 || (item.isReuseable && durability <= 0))
        {
            Clear();
        }

    }
    public void ActivateCraft()
    {
        //Tell the Game Manager to accept this item.  and if there is space remove this item
        if (GameManager.GetGameManager().AddToPlayerInventory(item))
        {
            RemoveOne();
        }
        else
        {
            //Tell the Player that you cannot craft this because you have no space. 
            GameManager.GetGameManager().m_playerController.Say("There is not enough room in my inventory to craft this");
        }

    }
}
