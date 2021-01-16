using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Animator))]
public class TreeCutting : UIInteractable
{

    //Reference to the inventory item
    [SerializeField] protected InventoryItem item;
    [SerializeField] private int numberOfHits;
    private int hitCount = 0;
    [SerializeField] private int amountPerHit;
    [SerializeField] private int amountWhenFell;
    
    //animator
    private Animator animator;

    protected override void Activate(string message)
    {
        InteractionTypes action = InteractionTypes.Unused;
        if (System.Enum.IsDefined(typeof(InteractionTypes), message))
        {
            action = (InteractionTypes)System.Enum.Parse(typeof(InteractionTypes), message);
        }



        switch (action)
        {
            case InteractionTypes.Use:
                if (hitCount < numberOfHits)
                {
                    int amount = amountPerHit;
                    hitCount++;
                    if(hitCount == numberOfHits)
                    {
                        amount = amountWhenFell;
                        animator.SetBool("Cut", true);
                    }
                    else
                    {
                        animator.SetTrigger("Chop");
                    }
                    //Send a message to the Game Manager to take the object
                    GameManager.GetGameManager().AddToPlayerInventory(item, amount);
                }
                if(hitCount == numberOfHits)
                {
                    GetComponent<BoxCollider2D>().enabled = false;
                }
                break;
            default:

                break;
        }
    }




    // Start is called before the first frame update
    void Start()
    {
        Setup();
        animator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {

    }
}
