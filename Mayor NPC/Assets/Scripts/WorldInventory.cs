using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(InventorySystem))]
public class WorldInventory : MonoBehaviour
{
    private SpriteRenderer renderer;
    [SerializeField] private InventoryObject inventoryObject;
    [SerializeField]private Canvas inventoryCanvas;
    // Number of inventory Slots
    [SerializeField] InventorySystem inventorySystem;
    // Start is called before the first frame update
    void Start()
    {
        if(inventoryObject == null)
        {
            Debug.LogError(gameObject.name + " does not have a required Inventory Object scriptable object assigned");
        }
        renderer = gameObject.GetComponent<SpriteRenderer>();

    }
    private void Reset()
    {
        renderer = gameObject.GetComponent<SpriteRenderer>();
        renderer.sprite = inventoryObject.closedArt;
        inventorySystem = GetComponent<InventorySystem>();
    }
    
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
