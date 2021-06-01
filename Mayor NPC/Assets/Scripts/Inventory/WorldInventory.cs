using System.Collections;
using UnityEngine;
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(InventorySystem))]
public class WorldInventory : MonoBehaviour
{
    private SpriteRenderer m_renderer;
    [SerializeField] private InventoryObject inventoryObject;
    [SerializeField] private Canvas inventoryCanvas;
    // Number of inventory Slots
    [SerializeField] InventorySystem inventorySystem;
    private bool isMouseOver;
    private bool isMouseOff;

    // Start is called before the first frame update
    void Start()
    {
        if (inventoryObject == null)
        {
            Debug.LogError(gameObject.name + " does not have a required Inventory Object scriptable object assigned");
        }
        m_renderer = gameObject.GetComponent<SpriteRenderer>();

    }
    private void Reset()
    {
        m_renderer = gameObject.GetComponent<SpriteRenderer>();
        m_renderer.sprite = inventoryObject.closedArt;
        inventorySystem = GetComponent<InventorySystem>();
    }


    // Update is called once per frame
    void Update()
    {

    }
    private void OnMouseEnter()
    {
        //inventoryCanvas.gameObject.SetActive(true);
        isMouseOver = true;

    }
    private void OnMouseOver()
    {
        if (isMouseOff)
        {
            StopCoroutine("MouseOff");
            isMouseOff = false;
        }
    }
    private void OnMouseExit()
    {
        OnMouseOff();
    }
    public void OnMouseOn()
    {
        OnMouseOver();
    }
    public void OnMouseOff()
    {
        StartCoroutine("MouseOff");
    }

    //mouse off Coroutine
    IEnumerator MouseOff()
    {
        isMouseOff = true;
        yield return new WaitForSeconds(.5f);
        //inventoryCanvas.gameObject.SetActive(false);
        isMouseOff = false;

    }
}
