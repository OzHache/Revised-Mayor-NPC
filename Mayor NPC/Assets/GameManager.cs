using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(InventorySystem))]
public class GameManager : MonoBehaviour
{
    //Reference to the player
    private GameObject Player;
    //Reference to the Inventory System
    InventorySystem inventorySystem;
    // Start is called before the first frame update
    private void Reset()
    {
        if (Player == null)
        {
            if (!GameObject.FindGameObjectWithTag("Player"))
            {
                Debug.LogError("There is no Tagged Player");

            }
            else
            {
                Player = GameObject.FindGameObjectWithTag("Player");
            }
        }
            

    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
