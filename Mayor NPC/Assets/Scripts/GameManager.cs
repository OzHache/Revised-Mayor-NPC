using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(InventorySystem))]
public class GameManager : MonoBehaviour
{
    private static GameManager gameManager;
    //Reference to the player
    public GameObject Player { get; private set; }
    //Reference to the Inventory System
    public InventorySystem playerInventory;
    //Static reference
    public static GameManager GetGameManager()
    {
        if(gameManager == null)
        {
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        }
        return gameManager;
    }

    //Event Handlers
    public delegate void NewDay();
    public static event NewDay NewDayEvent; 


    internal void PlayerSleep(bool isSafe)
    {
        bool enemiesPreset = false;
        //See if there are enemies around
        EnemySpawner[] spawners = FindObjectsOfType<EnemySpawner>();
        foreach(EnemySpawner spawner in spawners)
        {
            int numberOfEnemies = 0;
            if (spawner.EnemiesActive(out numberOfEnemies))
            {
                enemiesPreset = true;
                break;
            }
            spawner.Restart();
        }
        Player.GetComponent<PlayerController>().Sleep(isSafe: isSafe, enemiesPreset: enemiesPreset);

        //if there were enemies present, send all buildings a raid chance.
        //todo: make a building manager that handles all the buildings so the buildings are raided once only.
        if (enemiesPreset)
        {
            //BuildingManager.GetManager().Raid(numberOfEnemies:numberOfEnemies);
        }

        if (NewDayEvent != null)
        {
            NewDayEvent();
        }
    }

    
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

    // Update is called once per frame
    void Update()
    {

    }

    public void AddToPlayerInventory(InventoryItem addItem, int amount = 1)
    {
        playerInventory.AddToInventory(addItem, amount);

    }
}
