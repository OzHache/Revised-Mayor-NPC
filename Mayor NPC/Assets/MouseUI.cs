using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class MouseUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public LayerMask interactableLayer = 0;
    private int layerBitShift { get { return 1 << interactableLayer; } }
    //Reference to the current on screen position
    private Vector2 currentPosition = Vector2.zero;
    //reference to the Canvas
    private GameObject canvas;

    //References to the player
    private GameObject player = null;
    //if the UI isActive
    public bool isUIActive;

    // Start is called before the first frame update

    void Start()
    {
        //Set the reference to the canvas object and disable it
        canvas = transform.GetChild(0).gameObject;
        canvas.SetActive(false);
        //Set References to the Player

        if (!GameObject.FindGameObjectWithTag("Player"))
        {
            Debug.LogError("No Player Found", this.gameObject);
            Destroy(this, 0f);
            return;
        }
        player = GameObject.FindGameObjectWithTag("Player");

    }

    // Update is called once per frame
    void Update()
    {
        //Set the position of the UI
        SetUIPosition();
        //See if the UI needs to stay active
        SetUIActive();
        //Check for clicks
        if (Input.GetMouseButtonDown(0))
        {
            if (!canvas.activeInHierarchy)
            {
                player.SendMessage("Move", GetMousePosition(), SendMessageOptions.DontRequireReceiver);
            }
        }
        
    }

    private void SetUIActive()
    {
        if (canvas.activeInHierarchy)
        {
            //Check the distance bettween the mouses current position and the UI
            if (Vector2.Distance(GetMousePosition(), currentPosition) > 2.75f)
            {
                canvas.SetActive(false);
            }
        }
    }

    private void SetUIPosition()
    {
        //See if there is somethign interactable there
        RaycastHit2D hit = Physics2D.Raycast(GetMousePosition() - Vector2.up, Vector2.up, .75f);
        if (hit)
        {
            //We have hit something so set this as the position.
            Debug.Log(hit.transform.name);
            //Move the UI so it can be over elements in the screen
            transform.position = GetMousePosition();
            currentPosition = transform.position;
            canvas.SetActive(true);
        }
    }

    private Vector2 GetMousePosition()
    {
        // get the pixel position of the mouse in the world, convert it to a grid location
        Vector2 pixelPosition = Input.mousePosition;
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(pixelPosition);
        Vector2 mousePos = new Vector2((int)worldPos.x, (int)worldPos.y) + Vector2.one / 2;

        return mousePos;

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up *.5f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isUIActive = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isUIActive = false;
    }
}
