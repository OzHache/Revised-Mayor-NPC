using UnityEngine;


/// <summary>
/// the beacon needs to operate like the Building. You should be able to add "Oil" to the beacon and then activate it to get NPC's to show up
/// </summary>
public class Beacon : UIInteractable
{
    //Beacon Properties
    [SerializeField] private InventoryCell m_inventoryCell;

    private int m_currentAmountNeeded = -1;
    private bool m_isNeededItemsSubmitted = false;
    //Editor Properties
    [SerializeField] private InventoryItem m_requiredItem;
    [SerializeField] private int m_amountOfRequiredItem;
    [SerializeField] private GameObject m_inventoryCanvas;
    protected override void Activate(string message)
    {
        InteractionTypes action = InteractionTypes.Unused;
        if (System.Enum.IsDefined(typeof(InteractionTypes), message))
        {
            action = (InteractionTypes)System.Enum.Parse(typeof(InteractionTypes), message);
        }

        //the only action a Beacon should have is Use but if there is more interactions added they can be added later here.

        switch (action)
        {
            case InteractionTypes.Add:
                Debug.Log("Add");
                // clear the inventory cells
                m_inventoryCell.Remove(m_currentAmountNeeded);
                m_inventoryCell.Clear();
                //change the available interactions 
                RemoveInteraction(InteractionTypes.Add);
                AddInteraction(InteractionTypes.Use);
                //Set the current amount needed to -1
                m_currentAmountNeeded = -1;
                m_isNeededItemsSubmitted = true;
                break;
            case InteractionTypes.Use:
                //send a message to the Quest Manager to update this quest as completed
                PlayerActions playerAction;
                playerAction.m_action = Quest.ActionType.Use;
                playerAction.m_keyWord = "Beacon";
                playerAction.m_number = 1;
                QuestManager.GetQuestManager().UpdateQuests(playerAction);
                //can only be interacted wth once. 
                m_isInteractable = false;
                RemoveInteraction(InteractionTypes.Use);
                break;
        }
    }


    // Start is called before the first frame update
    void Start()
    {

    }
    private void UpdateInteractions()
    {
        if (m_isInteractable)
        {
            CheckRequiredItems();
        }
    }
    /// <summary>
    /// Check on the required items to see if there is an amount that has been met
    /// </summary>

    private void CheckRequiredItems()
    {
        if (m_inventoryCell.numberOfItems <= 0 && !interactions.Contains(InteractionTypes.Use))
        {
            AddInteraction(InteractionTypes.Use);
            m_inventoryCanvas.SetActive(false);
        }
    }



    // Update is called once per frame
    void Update()
    {
        if (m_currentAmountNeeded == -1 && !m_isNeededItemsSubmitted)
        {
            UpdateAmountNeeded();
        }
        UpdateInteractions();

    }
    /// <summary>
    /// Update the amount needed based on howmany itmes that we still require
    /// </summary>
    private void UpdateAmountNeeded()
    {
        m_currentAmountNeeded = m_amountOfRequiredItem;
        m_inventoryCell.LockInventory(m_requiredItem, m_amountOfRequiredItem);

    }

}
