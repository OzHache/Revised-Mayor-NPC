using Assets.Scripts.Agent_Scripts;
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
    public float getStamina { get { return m_maxStamina - m_usedStamina; } }
    public float getMaxStamina { get { return m_maxStamina; } }
    //References
    private Vector2 m_moveDirection = Vector2.zero;
    private Rigidbody2D m_rb;
    
    private PlayerDialogueController m_dialgoueController;

    private bool m_isMoving = false;
    private bool m_isAttacking = false;
    private Combatant m_combatant;

    

    //Updaters
    //Stamina Updater
    public delegate void Stamina();
    public static event Stamina StaminaUpdater;

    //Health Updater
    public delegate void Health();
    public static event Stamina HealthUpdater;

    //Calculated value of the position in 2D space
    private Vector2 position { get { return (Vector2)transform.position; } }

    // Start is called before the first frame update
    void Awake()
    {
        //Set References
        m_combatant = GetComponent<Combatant>();
        m_rb = GetComponent<Rigidbody2D>();
        m_dialgoueController = GetComponent<PlayerDialogueController>();
    }

    // Update is called once per frame
    void Update()
    {
        //Get input
       GetInput();
        if(m_moveDirection != Vector2.zero){
            StopAllCoroutines();
            m_isAttacking = false;
        }
    }
    private void FixedUpdate()
    {
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
    }

    //Get the input values
    private void GetInput()
    {
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

    //Moving
    IEnumerator MoveToTarget(Combatant combatant)
    {
        StopCoroutine("MoveToPosition");
        m_isAttacking = true;
        Vector2 enemyPos = combatant.transform.position;
        while (Vector2.Distance(transform.position, enemyPos) > this.m_combatant.meleeRange)
        {
            transform.position = Vector2.MoveTowards(position, enemyPos, m_speed * Time.deltaTime);
            yield return null;
            enemyPos = combatant.transform.position;
        }
        m_isAttacking = false;
        m_isMoving = false;
        this.m_combatant.Melee(combatant);
        StaminaUpdate(1);

    }
    private void StaminaUpdate(float amount)
    {
        m_usedStamina  = Mathf.Clamp(m_usedStamina += amount, 0, m_maxStamina);

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
    public float GetMaxHealth()
    {
        float maxHealth = m_combatant.getMaxHealth;
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
    

}

