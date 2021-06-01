using UnityEngine;

public class Shroom : UIInteractable
{
    //Reference to the inventory item
    [SerializeField] protected InventoryItem item;
    [SerializeField] protected float staminaValue;
    [SerializeField] private Quest m_quest;

    protected override void Activate(string message)
    {
        InteractionTypes action = InteractionTypes.Unused;
        if (System.Enum.IsDefined(typeof(InteractionTypes), message))
        {
            action = (InteractionTypes)System.Enum.Parse(typeof(InteractionTypes), message);
        }



        switch (action)
        {
            case InteractionTypes.Use:
                GameManager.GetGameManager().m_playerController.AddStamina(staminaValue);
                Destroy(gameObject);
                break;
            case InteractionTypes.Take:
                Take(item);
                break;
            default:

                break;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        Setup();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
