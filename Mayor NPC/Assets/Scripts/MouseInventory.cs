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
    //System we dragged from and item being dragged
    private InventorySystem dragFromSystem;
    private InventoryItem item;

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
        if (Input.GetMouseButtonUp(0) && dragCanvas.gameObject.activeInHierarchy)
        {
            MouseDrop();
        }
    }
    private void MouseDrop()
    {
        // drop this item in the world at position.
    }
    //Set the dragUI Position
    private void SetDragUI()
    {
        if (dragCanvas.gameObject.activeInHierarchy)
        {
            dragCanvas.transform.position = mouseUI.GetMousePosition(dragCanvas.scaleFactor);
        }
    }
    #region IconDrag
    public void ActivateIcon(Sprite sprite)
    {
        //todo: make this add the spirte of the dragable object and then drop it onto another inventory object that will accept it or send it back OR drop on the ground
        dragIconRect.GetComponent<Image>().sprite = sprite;
        dragIconRect.gameObject.SetActive(true);
        dragCanvas.gameObject.SetActive(true);
    }

    internal Vector3 DeactivateIcon()
    {
        dragIconRect.gameObject.SetActive(false);
        dragCanvas.gameObject.SetActive(false);
        return mouseUI.GetMousePosition();
    }
    #endregion IconDrag
}
