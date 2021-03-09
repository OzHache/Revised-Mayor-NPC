using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Take Two Crating items and convert them into a crafted Item
public class CraftingStation : MonoBehaviour
{
    //List of Recipies (Scriptable object that contains 2 other scriptable objects as a input and a single item as an output
    [SerializeField] private List<Recipie> recipies = new List<Recipie>();

    //two inventory Cells
    [SerializeField] private InventoryCell leftCell;
    [SerializeField] private InventoryCell rightCell;
    

    //Output Cell
    [SerializeField] private CraftingOutput outputCell;


    //Craft Monitor
    private bool isCrafting;
    private Recipie currentRecipie;

    private void Update()
    {
        if(leftCell.item != null && rightCell.item != null)
        {
            isCrafting = true;
        }

        if (isCrafting && currentRecipie == null)
        {
            CheckForValidRecipie();
        }
        else if (currentRecipie != null)
        {
            //currentRecipie = null;
        }
        
    }

    private void CheckForValidRecipie()
    {
        foreach(Recipie recipie in recipies)
        {
            if(recipie.ValidateRecipie(leftCell.item, rightCell.item))
            {
                currentRecipie = recipie;
                UpdateUI();
                break;
            }
        }

    }

    private void UpdateUI()
    {
        outputCell.AddItem(currentRecipie.output);
    }

    //When this is activated

    public void Activated()
    {
        //if this is a valid recipie
        if(currentRecipie != null)
        {
            leftCell.RemoveOne();
            rightCell.RemoveOne();
            currentRecipie = null;
        }
        //remove one from both 
    }
}
