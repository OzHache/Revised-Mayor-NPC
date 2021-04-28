using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldLady : Villager
{
    
    //Probably shouldn't be on the old lady 
    private GameLoop gameLoop = new GameLoop();
    // Start is called before the first frame update
    void Start()
    {
        m_characterDialogue = GetComponentInChildren<CharacterDialogue>();
        gameLoop.Initialize();

        //start the quest
        //tell the Dialogue attached here to set an action to end of dialogue
        m_characterDialogue.AddEndOfDialogueAction(() => StartScene(0));
        Setup();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal void StartScene(int i)
    {
        //trigger the Quest for this scene
        var quests = gameLoop.GetQuest(i);
        if(quests != null)
        {
            foreach(var quest in quests)
            {
                //if this is the last quest set this up to trigger the end of this quest
                if(quests.IndexOf(quest)== quests.Count - 1)
                {
                    quest.SetAction(() => gameLoop.EndQuest(i));
                }
                QuestManager.GetQuestManager().AddQuest(quest);
            }
        }
        //Start the Quest
        gameLoop.StartQuest(i);
        Debug.Log("Start Scene "+i);
    }
}
