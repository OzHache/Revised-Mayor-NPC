using UnityEngine;

public class ConnectCells : InventoryCell
{
    //Other Cell Reference
    [SerializeField] private ConnectCells otherCell;

    //See if the other cell is calling this one to update
    private void CheckForUpdate()
    {
        isUpdating = true;
        if (!otherCell.isUpdating)
        {
            UpdateCell();
        }
    }

    internal override void Add(int amount = 1)
    {
        base.Add(amount);
        CheckForUpdate();

    }
    public override void Clear()
    {
        base.Clear();
        CheckForUpdate();
    }
    internal override void Remove(int currentAmountNeeded)
    {
        base.Remove(currentAmountNeeded);
        CheckForUpdate();
    }
    internal override void RemoveOne()
    {
        base.RemoveOne();
        CheckForUpdate();
    }
    public override void Use()
    {
        base.Use();
        CheckForUpdate();
    }
    internal override void AddItem(InventoryItem newItem, int amount = 1)
    {
        base.AddItem(newItem, amount);
        CheckForUpdate();
    }

    public bool isUpdating = false;



    private void Update()
    {
    }

    private void UpdateCell()
    {
        //set both cells to not updating
        isUpdating = false;
        otherCell.isUpdating = false;
        otherCell.item = item;
        otherCell.numberOfItems = numberOfItems;
        otherCell.UpdateUI();
        UpdateUI();

    }

    public override bool Equals(object other)
    {
        //compare if the other is a connected cell
        if (!(other is ConnectCells))
        {
            return false;
        }
        //Amount and type are the same
        ConnectCells otherCell = other as ConnectCells;
        if (item == otherCell.item && numberOfItems == otherCell.numberOfItems)
        {
            return true;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
