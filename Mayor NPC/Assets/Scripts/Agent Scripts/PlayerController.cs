
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]

public class PlayerController : MonoBehaviour
{
    //Player scalers
    [SerializeField] private float m_speed = 5;
    [SerializeField] private float m_maxStamina = 50;
    private float m_usedStamina = 0f;
    public float getStamina { get { return m_maxStamina + m_usedStamina; } }
    public float getMaxStamina { get { return m_maxStamina; } }
    //References
    private Vector2 m_moveDirection = Vector2.zero;
    private Rigidbody2D m_rb;
    
    private PlayerDialogueController m_dialgoueController;

    private Coroutine m_conversation;
    private Combatant m_combatant;
    private bool m_engagedInConversation;

    private Vector3 m_respawnLocation;


    //Updaters
    //Stamina Updater
    public delegate void Stamina();
    public static event Stamina StaminaUpdater;

    //Health Updater
    public delegate void Health();
    public static event Stamina HealthUpdater;

    //Calculated value of the position in 2D space
    private Vector2 position { get { return (Vector2)transform.position; } }

    void Awake()
    {
        //Set References
        m_combatant = GetComponent<Combatant>();
        m_rb = GetComponent<Rigidbody2D>();
        m_dialgoueController = GetComponent<PlayerDialogueController>();
        m_respawnLocation = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //y sort
        GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(transform.position.y) * -1 +50;
        //Get input
        GetInput();
        if(m_moveDirection != Vector2.zero){
            StopAllCoroutines();
        }
    }
    private void FixedUpdate()
    {
        if (GameManager.GetGameManager().isGamePaused)
        {
            m_rb.velocity = Vector2.zero;
            return;
        }
        //Move the player in the direction of travel
        var directionToMove = m_moveDirection.normalized * m_speed;
        m_rb.velocity = directionToMove;
    }

    internal void Sleep(bool isSafe, bool enemiesPreset)
    {
        if (!isSafe)
        {
            if (enemiesPreset)
                Debug.Log("lose 50% of goods");
            else
                Debug.Log("lose 25% of goods");
        }
        m_combatant.RestartCharacter();
        HealthUpdater();
        m_usedStamina = 0;
        StaminaUpdate(0);
        m_respawnLocation = transform.position;
    }

    //Get the input values
    private void GetInput()
    {
        //early out while engaged in a conversation
        if (m_engagedInConversation)
            return;
        //Controls -> wasd for movememt
        m_moveDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }
    
    public void Engage(GameObject engageWith)
    {
        //See if this is a combatant
        if (engageWith.GetComponent<Combatant>())
        {
            //Am I within Range?
            if (Vector2.Distance(transform.position, engageWith.transform.position) > m_combatant.meleeRange)
            {
                StartCoroutine(MoveToTarget(engageWith.GetComponent<Combatant>()));
                //Move towards target and strike them, otherwise, strike them
            }
            else
            {
                m_combatant.Melee(engageWith.GetComponent<Combatant>());
                //Adjust based on the type of weapon
                StaminaUpdate(1);
            }
        }
    }

    internal void TalkAbout(UIInteractable interactableItem)
    {
        m_dialgoueController.TalkAbout(interactableItem);
    }

    //say generic message
    internal void Say(string message) 
    {
        m_dialgoueController.Say(message);
    }

    //Moving
    IEnumerator MoveToTarget(Combatant combatant)
    {
        StopCoroutine("MoveToPosition");
        Vector2 enemyPos = combatant.transform.position;
        while (Vector2.Distance(transform.position, enemyPos) > this.m_combatant.meleeRange)
        {
            transform.position = Vector2.MoveTowards(position, enemyPos, m_speed * Time.deltaTime);
            yield return null;
            enemyPos = combatant.transform.position;
        }
        this.m_combatant.Melee(combatant);
        StaminaUpdate(-1);

    }

    internal void Killed()
    {
        transform.position = m_respawnLocation;
        UIManager.GetUIManager().DeathScreen();
        m_combatant.RestartCharacter();
        //Reset the used values
        HealthUpdater();
        m_usedStamina = 0;
        StaminaUpdate(0);

    }

    public void StaminaUpdate(float amount)
    {
        m_usedStamina  = Mathf.Clamp(m_usedStamina += amount,-m_maxStamina, 0);

        if(StaminaUpdater != null)
        {
            StaminaUpdater();
        }
    }

    //get the players health from the combatant
    public float GetHealth()
    {
        float health = m_combatant.healthRemaining;
        return health;
    }

    //Return the health from the combat base class
    public int GetMaxHealth()
    {
        int maxHealth = m_combatant.GetMaxHealth();
        return maxHealth;
    }

    public void TakeDamage()
    {
        HealthUpdater();
    }  
    
    internal void AddStamina(float amount)
    {
        StaminaUpdate(-amount);
    }
    
    internal void StartConversation()
    {
        m_conversation = StartCoroutine(ConversationLock());
        m_engagedInConversation = true;
    }
    internal void StopConversation()
    {
        m_engagedInConversation = false;
        StopCoroutine(m_conversation);
    }
    private IEnumerator ConversationLock()
    {
        yield return new WaitUntil(()=>!m_engagedInConversation);
    }

}

