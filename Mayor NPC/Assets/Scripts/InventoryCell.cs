using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
[RequireComponent(typeof(DragAndDrop))]
public class InventoryCell : MonoBehaviour
{
    //Refences to item
    public InventoryItem item { get; private set; }
    //If this inventory should only accept a certian type
    public InventoryItem onlyAccepts { get; private set; }
    public bool lockedInventory { get { return onlyAccepts != null; } }
    //Counter for how many items
    public int numberOfItems { get; private set; }
    private int durability = 0;
    private InventorySystem iSystem;
    //Refernces to UI Elements
    //Image in the Panel
    private Image image;
    private TextMeshProUGUI counter;
    private Slider durabiltySlider;
    

    public void Start()
    {
        //Add this cell to the inventory system.
        //GameManager.GetGameManager().playerInventory.AddInventoryCell(this);

        image = GetComponent<Image>();
        counter = GetComponentInChildren<TextMeshProUGUI>();
        durabiltySlider = GetComponentInChildren<Slider>();
        
         UpdateUI();
    }

    internal void AddOne()
    {
        numberOfItems++;
        UpdateUI();
    }
    internal void RemoveOne()
    {
        numberOfItems--;
        if (numberOfItems == 0 || (item.isReuseable && durability <= 0))
        {
            Clear();
        }
        UpdateUI();

    }

    public void Use()
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

    internal void AddItem(InventoryItem newItem)
    {
        this.item = newItem;
        numberOfItems = 1;
        UpdateUI();
        Debug.Log("A new item has been added" + item.name);
    }

    //Clears the UI
    public void Clear()
    {
        item = null;
        onlyAccepts = null;
        numberOfItems = 0;
        UpdateUI();
    }

    private void UpdateUI() {
        //if we have cleared the item
        if(item == null)
        {
            image.sprite = null;
            counter.text = "";
        }
        else
        {
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

    internal void Remove(int currentAmountNeeded)
    {
        numberOfItems -= currentAmountNeeded;
        UpdateUI();
    }
}
