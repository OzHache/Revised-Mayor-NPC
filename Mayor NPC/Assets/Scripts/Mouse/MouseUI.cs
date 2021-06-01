using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MouseUI : MonoBehaviour
{
    private static MouseUI s_mouseUI;
    [SerializeField] private LayerMask m_interactableLayer = 0;

    //Scalars
    //Range that the player can interact with objects
    [SerializeField] float m_interactableRange;
    [SerializeField] float m_identificationRange;

    //Reference to the current on screen position
    private Vector2 m_currentPosition = Vector2.zero;

    //reference to the Canvas
    [SerializeField] private Canvas m_canvas;

    //TextMseshPro
    [SerializeField] private TextMeshProUGUI m_descriptionTMP;
    [SerializeField] private TextMeshProUGUI m_action_OneTMP;
    [SerializeField] private TextMeshProUGUI m_action_TwoTMP;
    [SerializeField] private TextMeshProUGUI m_action_ThreeTMP;

    //References to the player
    private GameObject m_player { get { return GameManager.GetGameManager().Player; } }
    //Reference to the item in focus
    private UIInteractable m_focusItem = null;

    //if the UI isActive
    public bool m_isUIActive;
    private Dictionary<UIButtonValues, string> m_UIActions = new Dictionary<UIButtonValues, string>();
    private bool m_isMouseOverUI = false;

    MouseUI() { s_mouseUI = this; }

    public static MouseUI GetMouseUI()
    {
        if (s_mouseUI == null)
        {
            s_mouseUI = GameObject.Find("MouseObject").GetComponent<MouseUI>();
            if (s_mouseUI == null)
            {
                Debug.LogError("No MouseObject");
            }
        }
        return s_mouseUI;
    }

    private void Start()
    {
        //Set the reference to the canvas object and disable it   
        m_canvas.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //See if the UI needs to stay active
        if (m_canvas.gameObject.activeInHierarchy)
        {
            SetUIActive();
        }
        //Check for clicks
        if (Input.GetMouseButtonDown(0))
        {
            GetObjectAtMouse();
        }
    }

    public void GetObjectAtMouse()
    {
        //see if we can talk about this item
        if (m_focusItem != null)
        {
            float distance = Vector2.Distance(GetMousePosition(), m_focusItem.transform.position);
            if (distance < 0.25f)
            {
                GameManager.GetGameManager().m_playerController.TalkAbout(m_focusItem);
            }
        }
        //ray to this item to see if we can fight
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
        if (hit)
        {
            //See if this is a combatant
            if (hit.transform.GetComponent<Combatant>() && !hit.transform.CompareTag("Player"))
            {
                m_player.GetComponent<PlayerController>().Engage(hit.transform.gameObject);
                return;
            }
            return;
        }
        //see if we are over our focus object

        return;
    }

    //Manage when the mouse is over the UI
    internal void MouseOver() { m_isMouseOverUI = true; }

    internal void MouseExit() { m_isMouseOverUI = false; }

    private void SetUIActive()
    {
        //If the focus item is null don't perform this action
        if (m_focusItem == null)
        {
            return;
        }

        if (m_canvas.gameObject.activeInHierarchy)
        {
            //Check the distance bettween the mouses current position and the UI
            float mouseDistance = Vector2.Distance(GetMousePosition(), m_focusItem.transform.position);
            if (mouseDistance > 2.75f)
            {
                m_focusItem = null;
                m_canvas.gameObject.SetActive(false);
                return;
            }
            float playeryDistance = Vector2.Distance(m_player.transform.position, m_focusItem.transform.position);
            if (playeryDistance > m_identificationRange)
            {
                m_canvas.gameObject.SetActive(false);
                m_focusItem = null;
            }
            else if (playeryDistance > m_interactableRange)
            {
                if (!m_canvas.isActiveAndEnabled)
                {
                    m_canvas.enabled = true;
                }

                m_descriptionTMP.gameObject.SetActive(true);
                m_action_OneTMP.transform.parent.gameObject.SetActive(false);
                m_action_TwoTMP.transform.parent.gameObject.SetActive(false);
                m_action_TwoTMP.transform.parent.gameObject.SetActive(false);
            }
            else
            {
                if (!m_canvas.isActiveAndEnabled)
                {
                    m_canvas.enabled = true;
                }

                m_descriptionTMP.gameObject.SetActive(true);
                m_action_OneTMP.transform.parent.gameObject.SetActive(true);
                m_action_TwoTMP.transform.parent.gameObject.SetActive(true);
                m_action_TwoTMP.transform.parent.gameObject.SetActive(true);
            }
        }
    }

    private void SetUIPosition(Vector3 pos)
    {
        transform.position = pos;
        m_currentPosition = pos;
        m_canvas.gameObject.SetActive(true);
    }

    public bool MouseHover(GameObject hoverObject)
    {
        //See if this object is within the interactable range
        if (Vector2.Distance(hoverObject.transform.position, m_player.transform.position) > m_identificationRange)
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
        //see if there is a description
        foreach (MonoBehaviour monoBehaviour in hoverObject.GetComponentsInChildren<MonoBehaviour>())
        {
            if (monoBehaviour is UIInteractable)
            {

                m_focusItem = monoBehaviour as UIInteractable;

                m_UIActions = m_focusItem.Identify();
                string desc = "???";
                string action_1 = "";
                string action_2 = "";
                string action_3 = "";
                m_UIActions.TryGetValue(UIButtonValues.Description, out desc);
                m_UIActions.TryGetValue(UIButtonValues.Action_1, out action_1);
                m_UIActions.TryGetValue(UIButtonValues.Action_2, out action_2);
                m_UIActions.TryGetValue(UIButtonValues.Action_3, out action_3);
                m_descriptionTMP.text = desc;
                m_action_OneTMP.text = action_1;
                m_action_TwoTMP.text = action_2;
                m_action_ThreeTMP.text = action_3;
                break;
            }
        }
    }

    //Returns the World Location of the mouse
    public Vector2 GetMousePosition(float scale = float.MinValue)
    {
        if (scale == float.MinValue)
        {
            scale = m_canvas.scaleFactor;
        }
        // get the pixel position of the mouse in the world, convert it to a grid location
        Vector2 pixelPosition = Input.mousePosition;
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(pixelPosition) * scale;

        return worldPos;
    }

    #region Button Clicks
    //Buttons on the UI Canvas
    public void ActionOne() { SendAction(UIButtonValues.Action_1); }
    public void ActionTwo() { SendAction(UIButtonValues.Action_2); }
    public void ActionThree() { SendAction(UIButtonValues.Action_3); }

    //Send Action to Focus Item
    private void SendAction(UIButtonValues buttonValues)
    {
        //If this item is null don't send messages
        if (m_focusItem == null)
        {
            return;
        }

        string action;
        m_UIActions.TryGetValue(buttonValues, out action);
        if (action == null)
        {
            return;
        }

        m_focusItem.SendMessage("Activate", action);
        //Remoe UI and Deselect Focus Item
        m_canvas.gameObject.SetActive(false);
        m_focusItem = null;

        //todo:send a message to the player telling them which item they used

    }
    #endregion Button Clicks
}
