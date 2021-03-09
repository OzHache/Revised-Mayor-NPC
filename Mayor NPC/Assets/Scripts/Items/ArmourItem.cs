using UnityEngine;
[CreateAssetMenu(fileName = "New_Armour", menuName = "ArmourItem")]
public class ArmourItem : InventoryItem, IEquipment
{
    private EquipmentTypes equipmentType { get { return isHead ? EquipmentTypes.Head : EquipmentTypes.Body; } }
    [SerializeField] private Sprite heldArt;
    [Tooltip("Percent added to the chance to evade damage")]
    [SerializeField] private float armorAmount;
    [Tooltip("Minimum damage absorption")]
    [SerializeField] private float damageReduction;
    [Tooltip("Is this a head gear")]
    [SerializeField] private bool isHead;

    public EquipmentTypes GetEquipmentTypes()
    {
        return equipmentType;
    }
}

internal interface IEquipment
{
    EquipmentTypes GetEquipmentTypes(); 
}