using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beacon : UIInteractable
{
    protected override void Activate(string message)
    {
        InteractionTypes action = InteractionTypes.Unused;
        if (System.Enum.IsDefined(typeof(InteractionTypes), message))
        {
            action = (InteractionTypes)System.Enum.Parse(typeof(InteractionTypes), message);
        }

        //the only action a Beacon should have is Use but if there is more interactions added they can be added later here.

        switch (action)
        {
            case InteractionTypes.Use:
                //send a message to the Quest Manager to update this quest as completed
                PlayerActions playerAction;
                playerAction.m_action = Quest.ActionType.Use;
                playerAction.m_keyWord = "Beacon";
                playerAction.m_number = 1;
                QuestManager.GetQuestManager().UpdateQuests(playerAction);
                //can only be interacted wth once. 
                m_isInteractable = false;
                break;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
