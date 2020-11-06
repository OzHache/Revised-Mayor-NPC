using UnityEngine;
[CreateAssetMenu(fileName = "New_Recipie", menuName = "CraftingRecipie")]

public class Recipie : ScriptableObject
{
    public string itemName;
    public string description;
    public InventoryItem inputOne;
    public int inputOneAmount = 1;
    public InventoryItem inputTwo;
    public int inputTwoAmount = 1;
    public InventoryItem output;
    public int outAmount = 1;


    public bool ValidateRecipie(InventoryItem itemOne, InventoryItem itemTwo)
    {
        bool isValid = false;

        if(itemOne == inputOne && itemTwo == inputTwo)
        {
            isValid = true;
        }
        //check for reversed craft
        if(itemOne == inputTwo && itemTwo == inputOne)
        {
            isValid = true;
        }
        return isValid;
    }
}
