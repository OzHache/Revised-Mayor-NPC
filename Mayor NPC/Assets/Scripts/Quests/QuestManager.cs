using System;
using System.Collections.Generic;

internal class QuestManager
{
    private static QuestManager s_instance;
    private Dictionary<string, List<QuestLog>> m_Listeners = new Dictionary<string, List<QuestLog>>();

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
        //Remove this quest from the UI
        QuestUI.GetQuestUI().RemoveQuest(log.GetQuest());
    }

    //todo:

    /*
     * this should be triggered on the dialogue with the player any time we want to start an quest we can send a keyword and an action
     * 
     */
    public void CheckQuest(PlayerActions action)
    {

        foreach (var quest in m_availableQuests)
        {
            if (quest.GetKey() == action.m_keyWord && quest.GetAction() == action.m_action)
            {
                //move it to current quests
                m_currentQuests.Add(quest);
                m_availableQuests.Remove(quest);
                if (!quest.IsCompleted())
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
                break;
            }
            //see if this quest is already discoverd

        }

    }

    public void UpdateQuests(PlayerActions action)
    {
        //look over the available quests and see if there are any that are triggered by this keyword and action
        //early out if there are no listeners
        if (m_Listeners == null || m_Listeners.Count == 0) 
            return;
        if (m_Listeners.ContainsKey(action.m_keyWord))
        {
            foreach(var quest in m_Listeners[action.m_keyWord])
            {
                quest.UpdateQuest(action.m_action);
            }
        }
        //if there are no quests of this type, early out
        if (m_Listeners.ContainsKey(action.m_keyWord))
        {
            //remove completed Quest
            //todo: cannot remove from a dictonaried list
            var quests = m_Listeners[action.m_keyWord];
            foreach (var quest in quests)
            {
                List<QuestLog> toRemove = new List<QuestLog>();
                if (quest.GetQuest().IsCompleted())
                {
                    quests.Remove(quest);
                }
            }
        }
    }
}