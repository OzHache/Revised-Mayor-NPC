using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Events;
public class Quest
{
    public Quest(ActionType action, string keyword, int amount, Action onComplete = null)
    {
        m_action = action;
        m_keyWord = keyword;
        m_remaining = amount;
        OnTriggerEvents = onComplete;
        m_id = s_lastId;
        s_lastId++;
    }

    //used then created from a serialization
    public void Initialize() 
    {
        m_id = s_lastId;
        s_lastId++;
    }
    public void SetAction(Action action)
    {
        OnTriggerEvents = action;
    }

    private static int s_lastId = 0;
    public enum ActionType {
        [XmlEnum(Name = "Collect")]
        Collect,
        [XmlEnum(Name = "Build")]
        Build,
        [XmlEnum(Name = "Use")]
        Use,
        [XmlEnum(Name = "Craft")]
        Craft, 
        [XmlEnum(Name = "Kill")]
        Kill
    }
    public void AddChildrenQuest(Quest child)
    {
        m_children.Add(child);
    }
    [XmlEnumAttribute("action")]
    private ActionType m_action;
    [XmlAttribute("keyword")]
    private string m_keyWord;
    [XmlAttribute("number")]
    private int m_remaining;
    private bool m_completed = false;
    private bool m_discovered = false;
    private List<Quest> m_children;
    private Action OnTriggerEvents;
    private int m_id;
    public int GetId() { return m_id; }
    internal string GetKey() { return m_keyWord; }
    internal List<Quest> GetChildren(){ return m_children; }
    internal ActionType GetAction() { return m_action; }
    public bool IsCompleted() { return m_completed; }
    public bool CanDiscover() 
    {
        if (m_discovered)
        {
            return false;
        }
        else
        {
            m_discovered = true;
            return true;
        }
    }
    internal int UpdateQuest(ActionType action, int amount = 1)
    {
        //if this is completed then don't process any more updated
        if (m_remaining <= 0)
        {
            m_completed = true;
            return amount;
        }
        if(action == m_action)
        {
            m_remaining-=amount;
            m_completed = m_remaining <= 0;

        }
        if (m_completed && OnTriggerEvents!= null)
        {
            OnTriggerEvents.Invoke();
        }
        //return the amount remaining so extra can be applied to the next quest.
        return -m_remaining;
    }

    public string GetQuest()
    {
        return m_action.ToString()+" " + m_remaining.ToString() +" "+ m_keyWord;
    }
    internal void ForceComplete()
    {
        m_completed = true;
        if (m_completed && OnTriggerEvents != null)
        {
            OnTriggerEvents.Invoke();
        }
    }
}
struct PlayerActions
{
    public Quest.ActionType m_action;
    public string m_keyWord;
    public int m_number;
}
