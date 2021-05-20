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
    public PlayerController m_playerController { get; private set; }
    //Reference to the Inventory System
    public InventorySystem playerInventory { get; private set; }
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
    public delegate void Startday();
    public static event Startday StartDay;

    private void Start()
    {
        //UIManager.UIHasActivated += GamePause;
    }


    internal void PlayerSleep(bool isSafe)
    {
        bool enemiesPresent = false;
        //See if there are enemies around
        EnemySpawner[] spawners = FindObjectsOfType<EnemySpawner>();
        foreach(EnemySpawner spawner in spawners)
        {
            int numberOfEnemies = 0;
            if (spawner.EnemiesActive(out numberOfEnemies))
            {
                enemiesPresent = true;
                break;
            }
            spawner.Restart();
        }
        Player.GetComponent<PlayerController>().Sleep(isSafe: isSafe, enemiesPreset: enemiesPresent);

        //if there were enemies present, send all buildings a raid chance.
        //todo: make a building manager that handles all the buildings so the buildings are raided once only.
        if (enemiesPresent)
        {
            //BuildingManager.GetManager().Raid(numberOfEnemies:numberOfEnemies);
        }

        if (NewDayEvent != null)
        {
            NewDayEvent();
        }
        //start wait for new day
        StartCoroutine(WaitForNewDay());
    }

    internal void PauseAction(bool? willPause )
    {
        if (willPause == null)
        {
            willPause = !isGamePaused;
        }
        else if (willPause != isGamePaused)
        {
            Debug.Log("Game Paused");
            isGamePaused = !isGamePaused;
            //this should be listend to by all movement objects
        }
        if(GamePaused != null)
        {
            GamePaused();
        }
    }

    IEnumerator WaitForNewDay()
    {
        //for now, use a timer
        float seconds = 2;
        while (seconds > 0)
        {
            yield return null;
            seconds -= Time.deltaTime;
        }
        if (StartDay != null)
        {
            StartDay();
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
    /// <summary>
    /// Setup before the game
    /// </summary>
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
                m_playerController = Player.GetComponent<PlayerController>();
            }
        }
        playerInventory = GetComponent<InventorySystem>();
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
        //todo change this to look in the equipment
        List<ToolType> tools = playerInventory.GetTools();
        return tools;
    }




}
