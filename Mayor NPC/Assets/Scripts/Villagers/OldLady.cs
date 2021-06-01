using UnityEngine;

public class OldLady : Villager
{

    //Probably shouldn't be on the old lady 
    private readonly GameLoop gameLoop = new GameLoop();
    // Start is called before the first frame update
    void Start()
    {

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
        if (i == 0) //first time
        {
            m_characterDialogue.AddEndOfDialogueAction(null);
        }
        //Pause the game
        //trigger the Quest for this scene
        System.Collections.Generic.List<Quest> quests = gameLoop.GetQuest(i);
        if (quests != null)
        {
            foreach (Quest quest in quests)
            {
                //if this is the last quest set this up to trigger the end of this quest
                if (quests.IndexOf(quest) == quests.Count - 1)
                {
                    quest.SetAction(() => gameLoop.EndQuest(i));
                }
                QuestManager.GetQuestManager().AddQuest(quest);
            }
        }
        //Start the Quest
        gameLoop.StartQuest(i);
        Debug.Log("Start Scene " + i);
    }
}
