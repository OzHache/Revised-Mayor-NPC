using System;
using UnityEngine;
[CreateAssetMenu(fileName = "New_InvetoryObject", menuName = "InventoryObject")]
[RequireComponent(typeof(InventorySystem))]
public class InventoryObject:ScriptableObject
{
    public string itemName;
    public string description;
    public Sprite closedArt;
    public Sprite openArt;
}
