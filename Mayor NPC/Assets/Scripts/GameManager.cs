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
    public PlayerController playerController;
    //Reference to the Inventory System
    public InventorySystem playerInventory;
    //Make a list of items that have the UpdateUI Interface
    //private List<IUpdateUI> updateUIs = new List<IUpdateUI>();
    public PlayerStatUI stamina;
    //Public bool for if the game is paused
    public bool isGamePaused { get; private set; }

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
    public delegate void Pause();
    public static event Pause GamePaused;

    private void Start()
    {
        UIManager.UIHasActivated += GamePause;
    }


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
                playerController = Player.GetComponent<PlayerController>();
            }
        }
        playerInventory = GetComponent<InventorySystem>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool AddToPlayerInventory(InventoryItem addItem, int amount = 1)
    {
        //see if there is available space
        bool spaceAvailable = playerInventory.IsSpaceAvailable(addItem);

        if(spaceAvailable)
            playerInventory.AddToInventory(addItem, amount);

        return spaceAvailable;
    }

    private void GamePause()
    {
        isGamePaused = UIManager.isUIActive;
        //Activate all elements that need to be paused
        if(GamePaused != null)
        {
            GamePaused();
        }
    }
    /// <summary>
    /// Search the inventory for all tools
    /// </summary>
    /// <returns> a list of tool types in the inventory</returns>
    internal List<ToolType> GetTools()
    {
        List<ToolType> tools = playerInventory.GetTools();
        return tools;
    }
}
