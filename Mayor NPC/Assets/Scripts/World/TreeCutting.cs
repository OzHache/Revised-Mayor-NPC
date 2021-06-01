using UnityEngine;
[RequireComponent(typeof(Animator))]
public class TreeCutting : UIInteractable
{

    //Reference to the inventory item
    [SerializeField] protected InventoryItem m_item;
    [SerializeField] private int m_numberOfHits = 3;
    private int hitCount = 0;
    [SerializeField] private int m_amountPerHit = 1;
    [SerializeField] private int m_amountWhenFell = 3;
    [SerializeField] private AudioClip m_ChopSound;
    private int m_soundID;


    //animator
    private Animator animator;

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
                if (hitCount < m_numberOfHits)
                {
                    int amount = m_amountPerHit;
                    hitCount++;
                    if (hitCount == m_numberOfHits)
                    {
                        amount = m_amountWhenFell;
                        animator.SetBool("Cut", true);
                        GetComponent<CircleCollider2D>().enabled = false;
                    }
                    else
                    {
                        animator.SetTrigger("Chop");

                    }
                    SoundManager.GetSoundManager().PlaySound(m_soundID);
                    //Send a message to the Game Manager to take the object
                    MessageFactory.GetMessageFactory().CreateFloatingMessage("-1 STA", FloatingMessage.MessageCategory.k_Stamina, gameObject);
                    GameManager.GetGameManager().AddToPlayerInventory(m_item, amount);
                    //send stamina useage to player
                    GameManager.GetGameManager().m_playerController.StaminaUpdate(-1);
                }
                if (hitCount == m_numberOfHits)
                {
                    GetComponent<BoxCollider2D>().enabled = false;
                }
                break;
            default:

                break;
        }
    }




    // Start is called before the first frame update
    void Start()
    {
        Setup();
        animator = GetComponent<Animator>();
        m_soundID = SoundManager.GetSoundManager().RegisterSoundToAction(m_ChopSound);
    }

}
