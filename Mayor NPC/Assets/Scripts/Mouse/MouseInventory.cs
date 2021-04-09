using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Mouse UI is used as a temporary holding spot while the item is dragged from one inventory slot to another.
/// </summary>
public class MouseInventory : MonoBehaviour, IDropHandler
{
    //Static Reference to the Mouse UI
    private static MouseInventory s_mouseInvUI;
    public static MouseInventory GetMouseInvUI(){ return s_mouseInvUI;}
    //MouseUI
    private MouseUI m_mouseUI;

    //Canvas that holds the sprite for the item being dragged
    [SerializeField] private Canvas m_dragCanvas = null;
    [SerializeField] private RectTransform m_dragIconRect = null;
  
    //Cell we are dragging from
    private InventoryCell m_fromCell;
    //Cell we are over right now
    private InventoryCell m_hoverCell;

    // Start is called before the first frame update
    private void Start()
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
        m_dragCanvas.gameObject.SetActive(false);

        //Get the mouseUI on this gameobject
        m_mouseUI = gameObject.GetComponent<MouseUI>();
        //Deactivate the Rect
        m_dragIconRect.gameObject.SetActive(false);

    }

    // Update is called once per frame
    private void Update()
    {
        //If the dragCanvas is active setDrag Canvas
        if (m_dragCanvas.gameObject.activeInHierarchy)
        {
            SetDragUI();
        }
    }

    //Set the dragUI Position
    private void SetDragUI()
    {
        if (m_dragCanvas.gameObject.activeInHierarchy)
        {

            m_dragIconRect.transform.position = Input.mousePosition;
            //dragCanvas.transform.position = mouseUI.GetMousePosition(dragCanvas.scaleFactor);
        }
    }
    //The cell the mouse is over
    internal void SetActiveCell(InventoryCell cell = null){m_hoverCell = cell;}

    #region IconDrag
    internal void PickUp(InventoryCell cell)
    {
        
        m_fromCell = cell;
        //todo: make this add the spirte of the dragable object and then drop it onto another inventory object that will accept it or send it back OR drop on the ground
        m_dragIconRect.GetComponent<Image>().sprite = m_fromCell.item.art;
        //Set the Drag Canvas to active
        m_dragIconRect.gameObject.SetActive(true);
        m_dragCanvas.gameObject.SetActive(true);
    }

    internal void ClearInventory(bool wasDropped)
    {
        
        //remove one from the available
        //todo: eventually make this remove as many as we are dragging
        if (wasDropped)
        {
            m_fromCell.RemoveOne();
        }
        //Deactivate the Drag cell
        m_dragIconRect.gameObject.SetActive(false);
        m_dragCanvas.gameObject.SetActive(false);
        m_fromCell = null;
    }

    internal int GetNumberOfItems()
    {
        //return the number of items in the stack
        return 1;
    }  

    internal InventoryItem GetItem() {  return m_fromCell.item;  }

    public void OnDrop(PointerEventData eventData)
    {
        StartCoroutine(DropOnGround());
    }
    private IEnumerator DropOnGround()
    {
        yield return null;
        //see if the item is still in the inventory
        if(m_fromCell != null)
        {
            //Instantiate(m_fromCell.item.)
        }
    }
    #endregion IconDrag
}
