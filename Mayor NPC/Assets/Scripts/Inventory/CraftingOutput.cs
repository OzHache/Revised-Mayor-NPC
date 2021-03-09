using UnityEngine;
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
        if (numberOfItems == 0 || (item.isReuseable && durability <= 0))
        {
            Clear();
        }
        UpdateUI();
        craftingStation.Activated();

    }
}
