using System;
using System.Collections.Generic;
using System.Linq;
using QuestPairs = System.Collections.Generic.KeyValuePair<string, System.Collections.Generic.List<QuestLog>>;
internal class QuestManager
{
    private static QuestManager s_instance;
    private List<QuestPairs> m_Listeners = new List<QuestPairs>();

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
            //find the pair with the keyword
            foreach (var pair in m_Listeners)
            {
                if (pair.Key == keyWord)
                {
                    pair.Value.Add(log);
                    return;
                }
            }
            m_Listeners.Add(new QuestPairs(keyWord, new List<QuestLog>() { log }));
        }
    }

    internal void AddQuest(object p)
    {
        throw new NotImplementedException();
    }

    public void RemoveQuest(QuestLog log)
    {
        string keyWord = log.GetQuestKey();
        //look over all the listeners and see if any are looking for this keyword
        foreach(var pair in m_Listeners)
        {
            if (pair.Key == keyWord)
            {
                //if remove fails. I would like to know
                if(!pair.Value.Remove(log))
                {
                    UnityEngine.Debug.LogError("Log not found" + log.GetQuestKey());
                }
            }
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
                    //find the listeners for this particular keyword
                    foreach (var pair in m_Listeners)
                    {
                        //why am I adding in two locations
                        if (pair.Key == keyWord)
                        {
                            pair.Value.Add(log);
                            break;
                        }
                    }


                    m_Listeners.Add(new QuestPairs(keyWord, new List<QuestLog>() { log }));

                }
                break;
            }
            //see if this quest is already discoverd

        }

    }

    public void UpdateQuests(PlayerActions action)
    {
        int? index = null; ;
        //look over the available quests and see if there are any that are triggered by this keyword and action
        //early out if there are no listeners
        if (m_Listeners == null || m_Listeners.Count == 0) 
            return;
        foreach (var pair in m_Listeners)
        {
            if (pair.Key == action.m_keyWord)
            {
                foreach (var quest in pair.Value)
                {
                    quest.UpdateQuest(action.m_action, action.m_number);
                    if(quest.GetQuest().IsCompleted())
                    {
                        if(!index.HasValue)
                        {
                            index = m_Listeners.IndexOf(pair);
                        }
                    }
                }
                break;
            }
        }
        if (index.HasValue)
        {
            List<QuestLog> removeList = new List<QuestLog>();
            foreach (var quest in m_Listeners[index.Value].Value)
            {
                if (quest.GetQuest().IsCompleted())
                {
                    removeList.Add(quest);
                }
            }
            //replace the quest pair with this value
            foreach(var questLog in removeList)
            {
                //remove from the list and then distroy this item
                m_Listeners[index.Value].Value.Remove(questLog);
                //add children quest
                foreach(var child in questLog.GetQuest().GetChildren())
                {
                    child.SetDiscovered(true);
                    AddQuest(child);
                }
                questLog.CloseQuest();
                
            }
            
        }
    }
}