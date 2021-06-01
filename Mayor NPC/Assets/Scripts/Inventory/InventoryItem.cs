using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New_Item", menuName = "InventoryItem")]
public class InventoryItem : ScriptableObject
{
    public string itemName;
    public string description;

    public Sprite art;
    public bool isReuseable;
    public bool isConsumeable;
    public int durability { get { return durability; } private set { durability = value; } }
    public bool IsTool() { return toolType != ToolType.None; }

    [SerializeField] private ToolType toolType = ToolType.None;
    [SerializeField] private Recipie m_recipie;
    public Recipie GetRecipie() { return m_recipie; }
    public List<Quest> GetQuest()
    {
        return m_recipie.GetQuest();
    }
    public int Use()
    {
        if (isReuseable)
        {
            return int.MaxValue;
        }
        else if (isConsumeable)
        {
            return -1;
        }
        else
        {
            //it's durable
            durability--;
            return durability;
        }

    }


    internal ToolType GetToolType()
    {
        return toolType;
    }


}