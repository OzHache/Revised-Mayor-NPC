using System;
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

    public int Use()
    {
        if (isReuseable)
        {
            return int.MaxValue;
        }
        else if(isConsumeable)
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