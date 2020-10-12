using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Collectable : MonoBehaviour, IInteractable
{
    //Dictionary for interactions
    protected Dictionary<UIButtonValues, string> interactionDescriptions = new Dictionary<UIButtonValues, string>();
    //Description of the obejct todo: Probably delete this
    [SerializeField] protected string descriptionOfObject;
    //Reference to the inventory item
    [SerializeField] protected InventoryItem item;

    protected enum InteractionTypes
    {
        Take,
        Use,
        Misc,
        Unused
    }
    abstract protected void Activate(string action);

    [SerializeField] protected InteractionTypes[] interactions;

    abstract public Dictionary<UIButtonValues, string> Identify();


    abstract protected void Setup();



    //Fill the interactions Dictionary with "Name","methodName",The First Item is always"Description" "Description of the item"
    abstract protected void FillInteractionDescription(); 
}
//Located in Collectable.cs
public enum UIButtonValues
{
    Description,
    Action_1,
    Action_2,
    Action_3
}
