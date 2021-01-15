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
    public InventoryItem onlyAccepts { get; private set; }
    public bool lockedInventory { get { return onlyAccepts != null; } }
    //Counter for how many items
    public int numberOfItems { get; protected set; }
    private int durability = 0;
    private InventorySystem iSystem;
    //Refernces to UI Elements
    //Image in the Panel
    [SerializeField] private Image image;
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
        numberOfItems = 1;
        UpdateUI();
        Debug.Log("A new item has been added" + item.name);
    }

    //Clears the UI
    virtual public void Clear()
    {
        item = null;
        onlyAccepts = null;
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
    public void LockInventory (InventoryItem itemToLock)
    {
        onlyAccepts = itemToLock;
    }
    public void UnlockInventory()
    {
        onlyAccepts = null;
    }

    virtual internal void Remove(int currentAmountNeeded)
    {
        numberOfItems -= currentAmountNeeded;
        UpdateUI();
    }

    public void OnDrop(PointerEventData eventData)
    {
        InventoryItem dropItem = MouseInventory.GetMouseInvUI().hasItem;
        //see if the mouse inventory has an item
        if (dropItem != null)
        {
            if (dropItem == item)
            {
                int numberOfItems = MouseInventory.GetMouseInvUI().GetNumberOfItems();
                MouseInventory.GetMouseInvUI().ClearInventory(true);
                Add(numberOfItems);
            }
            else if((item == null))
            {
               
                int numberOfItems = MouseInventory.GetMouseInvUI().GetNumberOfItems();
                MouseInventory.GetMouseInvUI().ClearInventory(true);
                AddItem(dropItem, numberOfItems);
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
