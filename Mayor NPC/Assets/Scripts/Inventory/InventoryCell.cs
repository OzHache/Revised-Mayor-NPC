using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
[RequireComponent(typeof(DragAndDrop))]
public class InventoryCell : MonoBehaviour, IDropHandler
{
    //Refences to item
    public InventoryItem item;
    //If this inventory should only accept a certian type
    [SerializeField]private InventoryItem m_onlyAccepts;
    public InventoryItem m_lockedItem { get { return m_onlyAccepts; } }
    public bool lockedInventory { get { return m_onlyAccepts != null; } }
    //Counter for how many items
    public int numberOfItems { get; protected set; }
    protected int durability = 0;
    private InventorySystem iSystem;
    //Refernces to UI Elements
    //Image in the Panel
    [SerializeField] protected Image image;
    [SerializeField] private TextMeshProUGUI counter;
    [SerializeField] private Slider durabiltySlider;
    

    public void Start()
    {
        //Add this cell to the inventory system.
        //GameManager.GetGameManager().playerInventory.AddInventoryCell(this);

       
        //counter = GetComponentInChildren<TextMeshProUGUI>();
        //durabiltySlider = GetComponentInChildren<Slider>();
        
         UpdateUI();
    }

    virtual internal void Add(int amount = 1)
    {
        //if the items are locked
        if (lockedInventory)
            numberOfItems -= amount;
        else
            numberOfItems += amount;
        
        UpdateUI();
    }
    virtual internal void RemoveOne()
    {
        numberOfItems--;
        if (numberOfItems == 0 || (item.isReuseable && durability <= 0))
        {
            Clear();
        }
        UpdateUI();

    }

    virtual public void Use()
    {
        if (item.isConsumeable)
        {
            //take one away
        } else if (item.isReuseable)
        {
            numberOfItems = 1;
        } else
        {
            //The item is durable and set the value to the durability
            
        }
        if(numberOfItems == 0 || (item.isReuseable && durability <= 0))
        {
            Clear();
        }
        
        UpdateUI();
    }

    virtual internal void AddItem(InventoryItem newItem, int amount = 1)
    {
        //
        this.item = newItem;
        if(lockedInventory)
            numberOfItems -= amount;
        else
            numberOfItems = 1;

        UpdateUI();
        Debug.Log("A new item has been added" + item.name);
    }

    //Clears the UI
    virtual public void Clear()
    {
        item = null;
        m_onlyAccepts = null;
        numberOfItems = 0;
        UpdateUI();
    }

    virtual protected void UpdateUI() {
        //if we have cleared the item
        if(item == null)
        {
            image.enabled = false;
            counter.text = "";
        }
        else
        {
            image.enabled = true;
            image.sprite = item.art;
            counter.text = numberOfItems.ToString();
        }

        if (m_onlyAccepts != null)
        {
            image.enabled = true;
            // set the art
            image.sprite = m_onlyAccepts.art;
            //change the value we need
            counter.text = numberOfItems.ToString();
        }

        //Update the durability slider or the Counter
    }

    public void SetInventorySystem(InventorySystem inventorySystem)
    {
        iSystem = inventorySystem;
    }

    /// <summary>
    /// Lock and unlock the inventory with the item to lock
    /// </summary>
    /// <param name="itemToLock">InventoryItem to locok the inventory with</param>
    public void LockInventory (InventoryItem itemToLock, int amount = 0)
    {
        numberOfItems = amount;
        m_onlyAccepts = itemToLock;
        UpdateUI();

    }
    public void UnlockInventory()
    {
        m_onlyAccepts = null;
    }

    virtual internal void Remove(int currentAmountNeeded)
    {
        for (int i = 0; i < currentAmountNeeded; i++)
        {
            RemoveOne();
        }
        
        UpdateUI();
    }

    /// <summary>
    /// Reach into the Mouse Inventory and see if we can extract the item from it
    /// </summary>
    /// <param name="eventData"></param>
    public virtual void OnDrop(PointerEventData eventData)
    {
        InventoryItem dropItem = MouseInventory.GetMouseInvUI().GetItem();
        //see if the mouse inventory has an item
        if (dropItem != null)
        {
            int numberOfItems = MouseInventory.GetMouseInvUI().GetNumberOfItems();
            //If the item being dropped is the same or null
            if (dropItem == item || item == null)
            {
                
                MouseInventory.GetMouseInvUI().ClearInventory(true);
                if (item == null)
                {
                    AddItem(dropItem, numberOfItems);
                }
                else
                {
                    Add(numberOfItems);
                }
                //Generate a new action based on what we have done
                var action = new PlayerActions();
                action.m_keyWord = dropItem.name;
                action.m_number = 1;
                action.m_action = Quest.ActionType.Collect;
                QuestManager.GetQuestManager().UpdateQuests(action);
            }
           
            else
            {
                MouseInventory.GetMouseInvUI().ClearInventory(false);
            }
            UpdateUI();
            
        }
        else
        {
            MouseInventory.GetMouseInvUI().ClearInventory(false);
        }
    }
}
