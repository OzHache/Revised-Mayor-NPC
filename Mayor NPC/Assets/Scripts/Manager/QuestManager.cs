using System;
using System.Collections.Generic;

internal class QuestManager
{
    private static QuestManager s_instance;
    private Dictionary<string, List<QuestLog>> m_Listeners;

    private List<Quest> m_availableQuests = new List<Quest>();
    private List<Quest> m_currentQuests = new List<Quest>();
    private List<Quest> m_completed = new List<Quest>();
    public static QuestManager GetQuestManager()
    {
        if (s_instance == null)
            s_instance = new QuestManager();
        return s_instance;
    }

    private QuestManager()
    {
        if(s_instance != null)
        {
            s_instance = this;
        }
    }

    internal void Initialize(List<Quest> startingQuests)
    {
        m_availableQuests = startingQuests;
    }

    public void AddQuest(Quest quest)
    {

        //see if this quest is already discoverd
        if (quest.IsDiscovered() && !quest.IsCompleted())
        {
            string keyWord = quest.GetKey();
            var log = QuestUI.GetQuestUI().AddQuest(quest);
            if (m_Listeners.ContainsKey(keyWord))
            {
                m_Listeners[keyWord].Add(log);
            }
            else
            {
                m_Listeners[keyWord] = new List<QuestLog>() { log };
            }
        }
    }
    public void RemoveQuest(QuestLog log)
    {
        string keyWord = log.GetQuestKey();
        if (m_Listeners.ContainsKey(keyWord))
        {
            m_Listeners[keyWord].Remove(log);
        }
        else
        {
            UnityEngine.Debug.LogError("Log not found" + log.GetQuestKey());
        }
        //add the children as Available Quest
        foreach(var quest in log.GetQuest().GetChildren())
        {
            if (!m_availableQuests.Contains(quest))
            {
                m_availableQuests.Add(quest);
            }
        }
    }
    
    //todo:

    /*
     * this should be triggered on the dialogue with the player any time we want to start an quest we can send a keyword and an action
     * 
     */


    public void UpdateQuests(string Keyword, Quest.Action action)
    {
        //look over the available quests and see if there are any that are triggered by this keyword and action
        foreach(var quest in m_availableQuests)
        {
            if(quest.GetKey() == Keyword && quest.GetAction() == action)
            {
                //move it to current quests
                m_currentQuests.Add(quest);
                m_availableQuests.Remove(quest);
                QuestUI.GetQuestUI().AddQuest(quest);
            }
        }
        if (m_Listeners.ContainsKey(Keyword))
        {
            foreach(var quest in m_Listeners[Keyword])
            {
                quest.UpdateQuest(action);
            }
        }
    }
}