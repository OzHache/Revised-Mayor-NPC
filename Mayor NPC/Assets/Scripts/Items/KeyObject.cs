using UnityEngine;
using UnityEngine.Events;

public class KeyObject : UIInteractable
{
    [SerializeField] UnityEvent m_triggerWhenCollected;
    protected override void Activate(string _)
    {
        //This does not go into the inventory, instead it will unlock the correspoding Locked Item
        PlayerActions action;
        action.m_action = Quest.ActionType.Collect;
        action.m_keyWord = "Key";
        action.m_number = 1;
        QuestManager.GetQuestManager().UpdateQuests(action);
        m_triggerWhenCollected.Invoke();
        Destroy(gameObject);
    }
    private void Start()
    {
        Setup();
    }
}