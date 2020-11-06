using UnityEngine;
[CreateAssetMenu(fileName = "New_Weapon", menuName = "WeaponItem")]
public class WeaponItem : InventoryItem
{

    public Sprite heldArt;
    //Does it take ammo?
    [Range(0,1)]public float hitChance;
    public int maxDamage;
    public int minDamage;
    //Regular Hit calculation
    public int hitDamage { get { return (UnityEngine.Random.Range(minDamage, maxDamage)); } }
    public int critMultiplyer;
    public float coolDown;


        
}