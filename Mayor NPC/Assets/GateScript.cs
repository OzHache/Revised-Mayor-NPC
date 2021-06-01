using UnityEngine;

public class GateScript : MonoBehaviour
{
    private bool m_locked = true;
    public void Unlock()
    {
        m_locked = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if this is the player
        if (collision.gameObject.CompareTag("Player") && !m_locked)
        {
            GetComponent<Animator>().SetBool("Open", true);
            GetComponent<Collider2D>().enabled = false;
            //Update the Quest Manager
            PlayerActions action;
            action.m_action = Quest.ActionType.Unlock;
            action.m_keyWord = "Gate";
            action.m_number = 1;
            QuestManager.GetQuestManager().UpdateQuests(action);

            Destroy(this);


        }
    }
}
