using UnityEngine.EventSystems;

public class WorldInventoryCell : InventoryCell, IEndDragHandler
{
    private WorldInventory m_inventory;
    private new void Start()
    {
        m_inventory = GetComponentInParent<WorldInventory>();
        UpdateUI();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        m_inventory.OnMouseOff();
    }

    public void OnMouseDrag()
    {
        m_inventory.OnMouseOn();
    }
    private void OnMouseEnter()
    {
        m_inventory.OnMouseOn();
    }
    private void OnMouseExit()
    {
        m_inventory.OnMouseOff();
    }
    private void OnMouseOver()
    {
        m_inventory.OnMouseOn();

    }

}
