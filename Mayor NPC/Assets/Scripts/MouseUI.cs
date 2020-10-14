using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class MouseUI : MonoBehaviour
{
    public LayerMask interactableLayer = 0;
    //Reference to the current on screen position
    private Vector2 currentPosition = Vector2.zero;
    //reference to the Canvas
    private GameObject canvas;
    [SerializeField]private RectTransform iconRect; 
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
        iconRect.gameObject.SetActive(false);
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
                focusItem = null;
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
            //Debug.Log(hit.transform.name);
            //Make sure this is interactable
            if (hit.collider.gameObject.CompareTag("Interactable")){
                // Update the UI
                UpdateUI(hit.transform.gameObject);
                //Move the UI so it can be over elements in the screen
                transform.position = GetMousePosition();
                currentPosition = transform.position;
                canvas.SetActive(true);
                
            }
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


            }

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

    }
    #endregion Button Clicks

    #region IconDrag
    private void ActivateIcon(Sprite sprite)
    {
        //todo: make this add the spirte of the dragable object and then drop it onto another inventory object that will accept it or send it back OR drop on the ground

    }

    #endregion IconDrag
}
