using UnityEngine;


/// <summary>
/// Drop object will spawn the requested item at either the players location or the location passed in by the calling entity
/// </summary>
public class Dropper : MonoBehaviour
{
    private static Dropper s_instance;
    public GameObject m_droppedPrefab;
    public static Dropper GetDropper()
    {
        if (s_instance == null)
        {
            //see if there is one already in the scene
            s_instance = FindObjectOfType<Dropper>();
            if (s_instance == null)
            {
                Debug.LogError("Ensure there is a Dropper in the game before this is called");
                return null;
            }
        }
        return s_instance;
    }

    private void Start()
    {
        if (s_instance == null)
        {
            s_instance = this;
        }
        else
        {
            Debug.LogError("There can be only one Dropper");
            Destroy(this);
        }
    }

    public void Drop(InventoryItem item)
    {
        //get the players position
        Transform startDrop = GameManager.GetGameManager().Player.transform;
        //create the parent
        GameObject parent = Instantiate(new GameObject(), startDrop.position, startDrop.rotation);
        //add the dropped item to the parent
        GameObject droppedItem = Instantiate(m_droppedPrefab, parent.transform);
        //Set up the item to be dropped
        droppedItem.GetComponent<CraftSupplies>().SetupItem(item);
    }
}
