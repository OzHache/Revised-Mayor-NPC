using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class InventoryCell : MonoBehaviour
{
    //Refences to item
    public InventoryItem item;
    //Counter for how many items
    int numberOfItems;

    //Refernces to UI Elements
    //Image in the Panel
    private Image image;
    private TextMeshProUGUI counter;
    private Slider durabiltySlider;

    public void Start()
    {
        image = GetComponent<Image>();
        counter = GetComponentInChildren<TextMeshProUGUI>();
        durabiltySlider = GetComponentInChildren<Slider>();
        if (item != null)
        {
            UpdateUI();
        }
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
        
        UpdateUI();
    }

    //Clears the UI
    private void Clear()
    {
        item = null;
        numberOfItems = 0;
        UpdateUI();
    }

    private void UpdateUI() {
        //if we have cleared the item
        if(item == null)
        {
            image = null;
            counter.text = "";
        }
        else
        {
            image.sprite = item.art;
            counter.text = numberOfItems.ToString();
        }
    }

    
    
}
