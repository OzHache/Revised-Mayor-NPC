using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    //Player scalers
    [SerializeField] private float speed = 5;
    [SerializeField] private float maxStamina = 10;
    private float usedStamina = 0f;
    public float getStamina { get { return maxStamina - usedStamina; } }
    public float getMaxStamina { get { return maxStamina; } }
    //References
    Vector2 moveDirection = Vector2.zero;
    Rigidbody2D rb;

    private bool isMoving = false;
    private bool isAttacking = false;
    private Combatant combatant;

    internal void AddStamina(float amount)
    {
        StaminaUpdate(-amount);
    }

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
        combatant = GetComponent<Combatant>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //Get input
       GetInput();
        if(moveDirection != Vector2.zero){
            StopAllCoroutines();
            isAttacking = false;
        }
    }
    private void FixedUpdate()
    {
        //Move the player in the direction of travel
        var directionToMove = moveDirection.normalized * speed;
        rb.velocity = directionToMove;
    }
    internal void Sleep(bool isSafe, bool enemiesPreset)
    {
        if (!isSafe)
        {
            if (enemiesPreset)
            {
                Debug.Log("lose 50% of goods");
            }
            else
            {
                Debug.Log("lose 25% of goods");
            }
        }
    }

    //Get the input values
    private void GetInput()
    {
        //Controls -> wasd for movememt
        moveDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }
    
    public void Engage(GameObject engageWith)
    {
        //See if this is a combatant
        if (engageWith.GetComponent<Combatant>())
        {
            //Am I within Range?
            if (Vector2.Distance(transform.position, engageWith.transform.position) > combatant.meleeRange)
            {
                StartCoroutine(MoveToTarget(engageWith.GetComponent<Combatant>()));
                //Move towards target and strike them, otherwise, strike them
            }
            else
            {
                combatant.Melee(engageWith.GetComponent<Combatant>());
                //Adjust based on the type of weapon
                StaminaUpdate(1);
            }
        }
    }
    //Moving
    IEnumerator MoveToTarget(Combatant combatant)
    {
        StopCoroutine("MoveToPosition");
        isAttacking = true;
        Vector2 enemyPos = combatant.transform.position;
        while (Vector2.Distance(transform.position, enemyPos) > this.combatant.meleeRange)
        {
            transform.position = Vector2.MoveTowards(position, enemyPos, speed * Time.deltaTime);
            yield return null;
            enemyPos = combatant.transform.position;
        }
        isAttacking = false;
        isMoving = false;
        this.combatant.Melee(combatant);
        StaminaUpdate(1);

    }
    private void StaminaUpdate(float amount)
    {
        usedStamina  = Mathf.Clamp(usedStamina += amount, 0, maxStamina);

        if(StaminaUpdater != null)
        {
            StaminaUpdater();
        }
    }

    //get the players health from the combatant
    public float GetHealth()
    {
        float health = combatant.healthRemaining;
        return health;
    }

    //Return the health from the combat base class
    public float GetMaxHealth()
    {
        float maxHealth = combatant.getMaxHealth;
        return maxHealth;
    }

    public void TakeDamage()
    {
        
        HealthUpdater();
    }
}

