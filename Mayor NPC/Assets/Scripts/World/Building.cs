using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Super Class for all buildable Buildings. 
/// </summary>
public class Building : UIInteractable
{
    [SerializeField] private InventoryCell m_inventoryCell;
    //There are 3 stages to a building, 
    [SerializeField] private GameObject m_foundation;
    [SerializeField] private GameObject m_framing;
    [SerializeField] private GameObject m_completedBuilding;
    //Requirements for each stage of building
    [SerializeField] private InventoryItem[] foundationRequiredItems;
    [SerializeField] private int[] amountOfFoundationItems;
    [SerializeField] private InventoryItem[] framingRequiredItems;
    [SerializeField] private int[] amountOfFramingItems;

    private int currentAmountNeeded = -1;
    private bool isNeededItemsSubmitted = false;

    //Stages of building
    public enum BuildingStage { Foundation, Framing, Completed, Destroyed, Empty}
    //Current stage of building
    [SerializeField]protected BuildingStage currentStage;

    // Start is called before the first frame update
    void Start()
    {
        SetStage();
        Setup();
    }

    private void SetStage()
    {
        m_foundation.SetActive(false);
        m_framing.SetActive(false);
        m_completedBuilding.SetActive(false);
        GetComponent<Collider2D>().enabled = true;
        switch (currentStage)
        {
            case BuildingStage.Foundation:
                m_foundation.SetActive(true);
                break;
            case BuildingStage.Framing:
                m_foundation.SetActive(true);
                m_framing.SetActive(true);
                break;
            case BuildingStage.Completed:
                m_completedBuilding.SetActive(true);
                //update this quest
                PlayerActions action;
                action.m_action = Quest.ActionType.Build;
                action.m_keyWord = "Building";
                action.m_number = 1;
                QuestManager.GetQuestManager().UpdateQuests(action);
                break;
            default:
                GetComponent<Collider2D>().enabled = false;
                break;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (currentAmountNeeded == -1 && !isNeededItemsSubmitted)
        {
            UpdateAmountNeeded();
        }
        UpdateInteractions();
    }

    private void UpdateInteractions()
    {
        if (m_isInteractable)
        {
            CheckRequiredItems();
        }
    }

    private bool CheckRequiredItems()
    {
        //If the items have already been submited then this is interactable. 
        if (isNeededItemsSubmitted)
        {
            return true;
        }
        switch (currentStage)
        {
            case BuildingStage.Foundation:
                if (amountOfFoundationItems == null)
                {
                    if (!interactions.Contains(InteractionTypes.Build) && !interactions.Contains(InteractionTypes.Add))
                        AddInteraction(InteractionTypes.Add);
                    return true;
                }
                if (m_inventoryCell.numberOfItems <= 0)
                {
                    if(!interactions.Contains(InteractionTypes.Build) && !interactions.Contains(InteractionTypes.Add))
                    AddInteraction(InteractionTypes.Add);

                    return true;
                }
                break;
            case BuildingStage.Framing:
                if (amountOfFramingItems == null)
                {
                    if (!interactions.Contains(InteractionTypes.Build) && !interactions.Contains(InteractionTypes.Add))
                        AddInteraction(InteractionTypes.Add);
                    return true;
                }
                if (m_inventoryCell.numberOfItems <= 0)
                {
                    if (!interactions.Contains(InteractionTypes.Build) && !interactions.Contains(InteractionTypes.Add))
                        AddInteraction(InteractionTypes.Add);
                    return true;
                }
                break;
        }
        return false;
    }
    

    protected override void Activate(string message)
    {
        InteractionTypes action = InteractionTypes.Unused;
        if (System.Enum.IsDefined(typeof(InteractionTypes), message))
        {
            action = (InteractionTypes)System.Enum.Parse(typeof(InteractionTypes), message);
        }



        switch (action)
        {
            case InteractionTypes.Build:
                Debug.Log("Build");
                switch (currentStage)
                {
                    case BuildingStage.Foundation:
                        currentStage = BuildingStage.Framing;
                        RemoveInteraction(InteractionTypes.Build);
                        isNeededItemsSubmitted = false;
                        break;
                    case BuildingStage.Framing:
                        currentStage = BuildingStage.Completed;
                        RemoveInteraction(InteractionTypes.Build);
                        isNeededItemsSubmitted = false;
                        break;
                    default:
                        break;
                }
                break;
            case InteractionTypes.Add:
                // clear the inventory cells
                m_inventoryCell.Remove(currentAmountNeeded);
                m_inventoryCell.Clear();
                //change the available interactions 
                RemoveInteraction(InteractionTypes.Add);
                AddInteraction(InteractionTypes.Build);
                //Set the current amount needed to -1
                currentAmountNeeded = -1;
                isNeededItemsSubmitted = true;
                break;
            default:
                break;
        }
        SetStage();
    }
    private void UpdateAmountNeeded()
    {
        switch (currentStage)
        {
            case BuildingStage.Foundation:
                currentAmountNeeded = amountOfFoundationItems[0];
                m_inventoryCell.LockInventory(foundationRequiredItems[0], amountOfFoundationItems[0]);
                break;
            case BuildingStage.Framing:
                currentAmountNeeded = amountOfFramingItems[0];
                m_inventoryCell.LockInventory(framingRequiredItems[0], amountOfFramingItems[0]);
                break;
            default:
                break;
        }
    }

}