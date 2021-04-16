using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CraftSupplies: UIInteractable
{
    //Reference to the Inventory Item
    [SerializeField] protected InventoryItem item;

    private void Start()
    {
        Setup();
    }
    /// <summary>
    /// Set this to a parent level  and only override when needed
    /// </summary>
    /// <param name="message">The message we recieve from the Game Manager</param>
    protected override void Activate(string message)
    {
        //the only action to take is taken
        Take(item);

    }

}
