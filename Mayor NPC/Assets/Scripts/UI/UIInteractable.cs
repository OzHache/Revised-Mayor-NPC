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
    protected Dictionary<UIButtonValues, string> m_interactionDescriptions = new Dictionary<UIButtonValues, string>();

    //Dictionary for required tools
    protected Dictionary<ToolType, string> m_toolRequirements = new Dictionary<ToolType, string>();
    [SerializeField] protected ToolType m_interactionTool;

    public ToolType GetTool() { return m_interactionTool; }

    //Description of the object todo: Probably delete this
    [SerializeField] protected string m_descriptionOfObject;
    //To be Modified if the object can be NOT interactable.
    public bool m_isInteractable = true;

    //Activate is what triggers when this object is used, the Word that is passed in is the string that corresponds to an action driven by theUIBUttonValues
    abstract protected void Activate(string action);

    [SerializeField] protected List<InteractionTypes> interactions = new List<InteractionTypes>();

    virtual internal List<Quest> GetQuest() { return null; }

    public Dictionary<UIButtonValues, string> Identify()
    {
        List<ToolType> playerTools = GameManager.GetGameManager().GetTools();
        //check if this can be interacted with
        if (m_interactionTool != ToolType.None)
        {
            //if the tool is not available then cancel the interaction.
            if (!playerTools.Contains(m_interactionTool))
            {
                Dictionary<UIButtonValues, string> returnValue = new Dictionary<UIButtonValues, string>();
                returnValue.Add(UIButtonValues.Description, m_descriptionOfObject);
                return returnValue;
            }
        }
        return m_interactionDescriptions;
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
        //ySort
        GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(transform.position.y * 100f) * -1;
        //Make sure there is a description
        if (m_descriptionOfObject == null)
        {
            m_descriptionOfObject = "???";
        }
        m_interactionDescriptions.Clear();
        FillInteractionDescription();
        m_interactionDescriptions.Add(UIButtonValues.Description, m_descriptionOfObject);
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

    internal string GetDescription()
    {
        return m_descriptionOfObject;
    }

    //Fill the interactions Dictionary with "Name","methodName",The First Item is always"Description" "Description of the item"
    private void FillInteractionDescription()
    {
        
        var i = 0;
        foreach (InteractionTypes iType in interactions)
        {
            switch (i)
            {
                case 0:
                    //fill the first action
                    m_interactionDescriptions.Add(UIButtonValues.Action_1, iType.ToString());
                    break;
                case 1:
                    //Fill the second action
                    m_interactionDescriptions.Add(UIButtonValues.Action_2, iType.ToString());
                    break;
                case 2:
                    //fill the third action
                    m_interactionDescriptions.Add(UIButtonValues.Action_3, iType.ToString());

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
        if (!m_isInteractable)
        {
            return;
        }
       /* if (m_interactionDescriptions.Count == 0)
        {
            return;
        }*/
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
