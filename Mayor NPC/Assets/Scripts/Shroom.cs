using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shroom : UIInteractable
{
    //Reference to the inventory item
    [SerializeField] protected InventoryItem item;
    [SerializeField] protected float staminaValue;

    protected override void Activate(string message)
    {
        InteractionTypes action = InteractionTypes.Unused;
        if(System.Enum.IsDefined(typeof(InteractionTypes), message))
        {
            action = (InteractionTypes)System.Enum.Parse(typeof(InteractionTypes), message);
        }
        


        switch (action)
        {
            case InteractionTypes.Use:
                GameManager.GetGameManager().playerController.AddStamina (staminaValue);
                Destroy(gameObject);
                break;
            case InteractionTypes.Take:
                Debug.Log("Adding this item to the inventory");
                //Send a message to the Game Manager to take the object
                GameManager.GetGameManager().AddToPlayerInventory(item);
                Destroy(gameObject);
                break;
            default:

                break;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        Setup();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
