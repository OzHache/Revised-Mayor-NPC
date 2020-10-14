using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(InventorySystem))]
public class GameManager : MonoBehaviour
{
    //Static reference
    public static GameManager GetGameManager()
    {
        if(gameManager == null)
        {
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        }
        return gameManager;
    }
    private static GameManager gameManager;
    //Reference to the player
    private GameObject Player;
    //Reference to the Inventory System
    public InventorySystem playerInventory;
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
        playerInventory = GetComponent<InventorySystem>();


    }
    void Awake()
    {
        if (gameManager != null && gameManager != this)
        {
            //another instance of Game Manager is active.
            Debug.Log("Another instance of Game Manager is active.");
            Destroy(this);
            return;
        }
        gameManager = this;
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Player Inventory managment
    public void AddToPlayerInventory(InventoryItem addItem)
    {
        playerInventory.AddToInventory(addItem);
    }
}
