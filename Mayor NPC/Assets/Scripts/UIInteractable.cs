using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ToolType
{
    Axe, Shovel, Hoe, Misc, None
}

[RequireComponent(typeof(Collider2D))]

public abstract class UIInteractable : MonoBehaviour, IInteractable
{
    //Dictionary for interactions
    protected Dictionary<UIButtonValues, string> interactionDescriptions = new Dictionary<UIButtonValues, string>();

    //Dictionary for required tools
    protected Dictionary<ToolType, string> interactionToolRequirements = new Dictionary<ToolType, string>();
    [SerializeField] protected ToolType interactionTool;

    public ToolType GetTool() { return interactionTool; }

    //Description of the obejct todo: Probably delete this
    [SerializeField] protected string descriptionOfObject;
    //To be Modified if the object can be NOT interactable.
    public bool isInteractable = true;

    


    abstract protected void Activate(string action);

    [SerializeField] protected List<InteractionTypes> interactions = new List<InteractionTypes>();

    public Dictionary<UIButtonValues, string> Identify()
    {
        List<ToolType> playerTools = GameManager.GetGameManager().GetTools();
        //check if this can be interacted with
        if (interactionTool != ToolType.None)
        {
            //if the tool is not available then cancel the interaction.
            if (!playerTools.Contains(interactionTool))
            {
                Dictionary<UIButtonValues, string> returnValue = new Dictionary<UIButtonValues, string>();
                returnValue.Add(UIButtonValues.Description, descriptionOfObject);
                return returnValue;
            }
        }
        return interactionDescriptions;
    }
    #region Default Interactions
    protected void Take(InventoryItem item)
    {
        //this item is only taken into the inventory
        bool added = GameManager.GetGameManager().AddToPlayerInventory(item);
        if (added)
        {
            Destroy(gameObject);
        }
    }


    #endregion Default interactions
    protected void Setup()
    {
        //Make sure there is a description
        if (descriptionOfObject == null)
        {
            descriptionOfObject = "???";
        }
        interactionDescriptions.Clear();
        FillInteractionDescription();
        interactionDescriptions.Add(UIButtonValues.Description, descriptionOfObject);
    }
    protected void RemoveInteraction(InteractionTypes interaction)
    {
        interactions.Remove(interaction);
        Setup();
    }
    protected void AddInteraction(InteractionTypes interaction)
    {
        interactions.Add(interaction);
        Setup();
    }



    //Fill the interactions Dictionary with "Name","methodName",The First Item is always"Description" "Description of the item"
    private void FillInteractionDescription()
    {
       

        //todo: Convert these into strategies that can accomodate tools

        
        var i = 0;
        foreach (InteractionTypes iType in interactions)
        {
            switch (i)
            {
                case 0:
                    //fill the first action
                    interactionDescriptions.Add(UIButtonValues.Action_1, iType.ToString());
                    break;
                case 1:
                    //Fill the second action
                    interactionDescriptions.Add(UIButtonValues.Action_2, iType.ToString());
                    break;
                case 2:
                    //fill the third action
                    interactionDescriptions.Add(UIButtonValues.Action_3, iType.ToString());

                    break;
                default:
                    Debug.Log(i + " There is an extra itype beyond the allowed 3");
                    break;
            }
            i++;
        }
    }
    private void OnMouseEnter()
    {
        if (!isInteractable)
        {
            return;
        }
        if (interactionDescriptions.Count == 0)
        {
            return;
        }
        if (!MouseUI.GetMouseUI().MouseHover(gameObject))
        {
            //keep trying until it does pop up;
            StartCoroutine(MouseHovering());
        }
    }



    IEnumerator MouseHovering()
    {
        
        while (!MouseUI.GetMouseUI().MouseHover(gameObject))
        {
            yield return null;
            //Keep trying, you'll get better !!
        }
    }
}
//Located in UIIteractable.cs
public enum UIButtonValues
{
    Description,
    Action_1,
    Action_2,
    Action_3
}
