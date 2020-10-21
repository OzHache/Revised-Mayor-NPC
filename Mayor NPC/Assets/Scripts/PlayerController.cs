using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.EventSystems;


public class PlayerController : MonoBehaviour
{
    //Player scalers
    [SerializeField] private float speed = 5;

    //References
    private Vector2 destination = -Vector2.one;
    private bool isMoving = false;
    private bool isAttacking = false;
    private Combatant combat;

    //Calculated value of the position in 2D space
    private Vector2 position { get { return (Vector2)transform.position; } }

    // Start is called before the first frame update
    void Start()
    {
        //Set References
        combat = GetComponent<Combatant>();
    }

    // Update is called once per frame
    void Update()
    {
        //Get input
        GetInput();
        //If we can move, then move
        if (isMoving && !isAttacking)
        {
            Moving();
        }
    }
    //Handle movement between frames
    private void Moving()
    {
        Move(destination);
    }
    // Handle movement input from the Mouse
    public void Move(Vector2 newPosition)
    {
        Move(newPosition, true);
    }
    // Handle movement from the attack
    public void Move(Vector2 newPosition, bool fromMouse = true)
    {
        if (fromMouse)
        {
            StopCoroutine("MoveToTarget");
            isAttacking = false;
        }
        if (Vector2.Distance(newPosition, position) > .05f)
        {
            isMoving = true;
            destination = newPosition;
        }
        if (isMoving)
        {
            transform.position = Vector2.MoveTowards(position, destination, speed * Time.deltaTime);
            isMoving = Vector2.Distance(destination, position) > .05f;
        }
    }

    //Get the input values
    private void GetInput()
    {
     //
    }
    //Return the position of the mouse as a Integer Vector 2
    private Vector2 GetMousePosition()
    {
        // get the pixel position of the mouse in the world, convert it to a grid location
        Vector2 pixelPosition = Input.mousePosition;
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(pixelPosition);
        Vector2 mousePos = new Vector2((int)worldPos.x, (int)worldPos.y) + Vector2.one / 2;

        return mousePos;
    }
    public void Engage(GameObject engageWith)
    {
        //See if this is a combatant
        if (engageWith.GetComponent<Combatant>())
        {
            //Am I within Range?
            if (Vector2.Distance(transform.position, engageWith.transform.position) > combat.meleeRange)
            {
                StartCoroutine(MoveToTarget(engageWith.GetComponent<Combatant>()));
                //Move towards target and strike them, otherwise, strike them
            }
            else
            {
                combat.Melee(engageWith.GetComponent<Combatant>());
            }
        }
    }
    //Moving
    IEnumerator MoveToTarget(Combatant combatant)
    {
        isAttacking = true;
        Vector2 enemyPos = combatant.transform.position;
        while(Vector2.Distance(transform.position, enemyPos) > combat.meleeRange)
        {
            Move(enemyPos, fromMouse: false);
            yield return null;
            enemyPos = combatant.transform.position;
        }
        isAttacking = false;
        isMoving = false;
        combat.Melee(combatant);
    }
}
