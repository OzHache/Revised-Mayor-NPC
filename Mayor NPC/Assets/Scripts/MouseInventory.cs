using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseInventory : MonoBehaviour
{
    //Static Reference to the Mouse UI
    private static MouseInventory s_mouseInvUI;
    public static MouseInventory GetMouseInvUI()
    {
        return s_mouseInvUI;
    }
    //MouseUI
    private MouseUI mouseUI;

    //Canvas that holds the sprite for the item being dragged
    [SerializeField] private Canvas dragCanvas;
    [SerializeField] private RectTransform dragIconRect;

    //item being dragged
    private InventoryItem item;
    public InventoryItem hasItem { get { return fromCell.item; } }
    //Cell we are dragging from
    InventoryCell fromCell;
    //Cell we are dragging to
    InventoryCell toCell;
    //Cell we are over right now
    InventoryCell hoverCell;

    // Start is called before the first frame update
    void Start()
    {
        //Set the mouse ui static reference to this.
        if (s_mouseInvUI != null)
        {
            Debug.Log("There is alread a Mouse UI");
            Destroy(this);
        }
        else
        {
            s_mouseInvUI = this;
        }
        dragCanvas.gameObject.SetActive(false);

        //Get the mouseUI on this gameobject
        mouseUI = gameObject.GetComponent<MouseUI>();
        //Deactivate the Rect
        dragIconRect.gameObject.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        //If the dragCanvas is active setDrag Canvas
        if (dragCanvas.gameObject.activeInHierarchy)
        {
            SetDragUI();
        }
        
    }
    //Set the dragUI Position
    private void SetDragUI()
    {
        if (dragCanvas.gameObject.activeInHierarchy)
        {

            dragIconRect.transform.position = Input.mousePosition;
            //dragCanvas.transform.position = mouseUI.GetMousePosition(dragCanvas.scaleFactor);
        }
    }
    //The cell the mouse is over
    public void SetActiveCell(InventoryCell cell = null)
    {
        hoverCell = cell;
    }


    #region IconDrag
    public void PickUp(InventoryCell cell)
    {

        fromCell = cell;
        //todo: make this add the spirte of the dragable object and then drop it onto another inventory object that will accept it or send it back OR drop on the ground
        dragIconRect.GetComponent<Image>().sprite = fromCell.item.art;
        //Set the Drag Canvas to active
        dragIconRect.gameObject.SetActive(true);
        dragCanvas.gameObject.SetActive(true);
    }

    

    internal void ClearInventory()
    {
        //Deactivate the Drag cell
        dragIconRect.gameObject.SetActive(false);
        dragCanvas.gameObject.SetActive(false);
    }

    internal int GetNumberOfItems()
    {
        //return the number of items in the stack
        return 1;
    }

    internal InventoryItem GetItem()
    {
        return fromCell.item;
    }
    #endregion IconDrag
}
