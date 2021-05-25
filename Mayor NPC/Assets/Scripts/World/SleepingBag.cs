using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepingBag : UIInteractable
{
    private void Start()
    {
        Setup();
    }

    /// <summary>
    /// Sleep tell the GameManager that sleep is happening in a safe Space
    /// </summary>
    // Start is called before the first frame update
    public void Sleep()
    {
        GameManager.GetGameManager().PlayerSleep(isSafe: true);
    }

    protected override void Activate(string _)
    {
        PlayerActions action;
        action.m_action = Quest.ActionType.Use;
        action.m_keyWord = "SleepingBag";
        action.m_number = 1;
        QuestManager.GetQuestManager().UpdateQuests(action);
        Sleep();
    }
}
