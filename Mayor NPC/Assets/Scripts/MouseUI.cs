using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MouseUI : MonoBehaviour
{

    private static MouseUI mouseUI;

    public static MouseUI GetMouseUI()
    {
        if (mouseUI == null)
        {
            mouseUI = GameObject.Find("MouseObject").GetComponent<MouseUI>();
            if(mouseUI == null)
            {
                Debug.LogError("No MouseObject");
            }
        }
        return mouseUI;
    }
    
    public LayerMask interactableLayer = 0;
    //Reference to the current on screen position
    private Vector2 currentPosition = Vector2.zero;



    //reference to the Canvas
    [SerializeField]private Canvas interactionCanvas;
    
    //TextMseshPro
    [SerializeField]private TextMeshProUGUI descriptionTMP;
    [SerializeField]private TextMeshProUGUI action_OneTMP;
    [SerializeField]private TextMeshProUGUI action_TwoTMP;
    [SerializeField]private TextMeshProUGUI action_ThreeTMP;


    //References to the player
    private GameObject player = null;
    //Reference to the item in focus
    private GameObject focusItem = null;

    //if the UI isActive
    public bool isUIActive;
    Dictionary<UIButtonValues, string> UIActions = new Dictionary<UIButtonValues, string>();
    private bool isMouseOverUI = false;
    //Range that the player can interact with objects
    [SerializeField] float interactableRange;

    // Start is called before the first frame update

    void Start()
    {
        
        //Set the reference to the canvas object and disable it
        
        interactionCanvas.gameObject.SetActive(false);
        
        
        //Set References to the Player

        if (!GameObject.FindGameObjectWithTag("Player"))
        {
            Debug.LogError("No Player Found", this.gameObject);
            Destroy(this, 0f);
            return;
        }
        player = GameManager.GetGameManager().Player;
        
    }

    // Update is called once per frame
    void Update()
    {
        //Set the position of the UI
        //SetUIPosition();
        //See if the UI needs to stay active
        if (interactionCanvas.gameObject.activeInHierarchy)
            SetUIActive();
        //Check for clicks
        if (Input.GetMouseButtonDown(0))
        {
            GetObjectAtMouse();
        }
    }

    public GameObject GetObjectAtMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
        if (hit)
        {
            //See if this is a combatant
            if (hit.transform.GetComponent<Combatant>() && !hit.transform.CompareTag("Player"))
            {
                player.GetComponent<PlayerController>().Engage(hit.transform.gameObject);
                return null;
            }
            return hit.transform.gameObject;
            //detect what this object is
            //Manage if dragging
        }
        return null;
    }

    //Manage when the mouse is over the UI
    internal void MouseOver()
    {
        isMouseOverUI = true;
    }

    internal void MouseExit()
    {
        isMouseOverUI = false;
    }

    private void SetUIActive()
    {
        //If the focus item is null don't perform this action
        if (focusItem == null)
            return;
        if (interactionCanvas.gameObject.activeInHierarchy)
        {
            //Check the distance bettween the mouses current position and the UI
            if (Vector2.Distance(GetMousePosition(), currentPosition) > 2.75f)
            {
                interactionCanvas.gameObject.SetActive(false);
                focusItem = null;
            }else if(Vector2.Distance(player.transform.position, focusItem.transform.position) > interactableRange)
            { 
                interactionCanvas.gameObject.SetActive(false);
            } 
            else if(focusItem != null)
            {
                interactionCanvas.gameObject.SetActive(true);
            }
        }
    }

    private void SetUIPosition(Vector3 pos)
    {
        transform.position = pos;
        currentPosition = pos;
        interactionCanvas.gameObject.SetActive(true);
    }

    public bool MouseHover(GameObject hoverObject)
    {
        //See if this object is within the interactable range
        if (Vector2.Distance(hoverObject.transform.position, player.transform.position) > interactableRange)
        {
            //Activate the move in range UI
            return false;
        }
        else
        {
            UpdateUI(hoverObject);
            SetUIPosition(hoverObject.transform.position);
            return true;
        }
    }
    private void UpdateUI(GameObject hoverObject)
    {
        //set a reference to the Hover Object
        focusItem = hoverObject;
        //see if there is a description
        foreach(MonoBehaviour monoBehaviour in hoverObject.GetComponentsInChildren<MonoBehaviour>())
        {
            if(monoBehaviour is IInteractable)
            {
                
                var interactable = monoBehaviour as IInteractable;

                UIActions = interactable.Identify();
                string desc = "???";
                string action_1 = "";
                string action_2 = "";
                string action_3 = "";
                UIActions.TryGetValue(UIButtonValues.Description, out desc);
                UIActions.TryGetValue(UIButtonValues.Action_1, out action_1);
                UIActions.TryGetValue(UIButtonValues.Action_2, out action_2);
                UIActions.TryGetValue(UIButtonValues.Action_3, out action_3);
                descriptionTMP.text = desc;
                action_OneTMP.text = action_1;
                action_TwoTMP.text = action_2;
                action_ThreeTMP.text = action_3;

                break;

            }

        }
    }

    //Returns the World Location of the mouse
    public Vector2 GetMousePosition(float scale = float.MinValue)
    {
        if(scale == float.MinValue)
        {
            scale = interactionCanvas.scaleFactor;
        }
        // get the pixel position of the mouse in the world, convert it to a grid location
        Vector2 pixelPosition = Input.mousePosition;
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(pixelPosition)* scale;
        //Vector2 mousePos = new Vector2((int)worldPos.x, (int)worldPos.y) + Vector2.one / 2;

        return worldPos;

    }

    #region Button Clicks
    //Buttons on the UI Canvas
    public void ActionOne()
    {
        SendAction(UIButtonValues.Action_1);
    }
    public void ActionTwo()
    {
        SendAction(UIButtonValues.Action_2);
    }
    public void ActionThree()
    {
        SendAction(UIButtonValues.Action_3);
    }

    //Send Action to Focus Item
    private void SendAction(UIButtonValues buttonValues)
    {
        //If this item is null don't send messages
        if(focusItem == null)
        {
            return;
        }
        string action;
        UIActions.TryGetValue(buttonValues, out action);
        focusItem.SendMessage("Activate",action);
        //Remoe UI and Deselect Focus Item
        interactionCanvas.gameObject.SetActive(false);
        focusItem = null;

    }
    #endregion Button Clicks
}
