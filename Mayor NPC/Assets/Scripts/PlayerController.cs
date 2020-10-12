using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class PlayerController : MonoBehaviour
{
    //Player scalers
    [SerializeField] private float speed = 5;

    //References
    private Vector2 destination = -Vector2.one;
    private bool isMoving = false;

    //Calculated value of the position in 2D space
    private Vector2 position { get { return (Vector2)transform.position; } }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Get input
        GetInput();
        //If we can move, then move
        if (isMoving)
        {
            Moving();
        }
    }
    private void Moving()
    {
        Move(destination);
    }
    public void Move(Vector2 newPosition)
    {
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
}
