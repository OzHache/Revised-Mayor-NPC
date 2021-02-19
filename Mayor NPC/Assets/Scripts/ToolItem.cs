using UnityEngine;
[CreateAssetMenu(fileName = "New_Tool", menuName = "ToolItem")]
public class ToolItem : InventoryItem, IEquipment
{
    public EquipmentTypes equipmentType = EquipmentTypes.Tool;
    public Sprite heldArt;

    public float coolDown;

    public EquipmentTypes GetEquipmentTypes()
    {
        return equipmentType;
    }


}
