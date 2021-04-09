using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New_Quest", menuName = "Quests")]
public class Quest : ScriptableObject
{
    public enum Action { Collect, Build, Use, Craft }
    [SerializeField]private Action m_action;
    [SerializeField]private string m_keyWord;
    [SerializeField]private int m_remaining;
    [SerializeField] private int m_defaultAmount;
    [SerializeField]private bool m_completed = false;
    [SerializeField]private bool m_discovered = false;
    [SerializeField]private List<Quest> m_children;

    public void Reset()
    {
        m_remaining = m_defaultAmount;
        m_completed = false;
    }
    internal string GetKey() { return m_keyWord; }

    internal List<Quest> GetChildren()
    {
        return m_children;
    }
    internal Action GetAction() { return m_action; }

    public bool IsCompleted() { return m_completed; }

    internal void UpdateQuest(Action action)
    {
        if(action == m_action)
        {
            m_remaining--;
            m_completed = m_remaining <= 0;
        }
    }
    public void SetDiscovered(bool discovered) { m_discovered = discovered; }
    public bool IsDiscovered() { return m_discovered; }

    public string GetQuest()
    {
        return m_action.ToString()+" " + m_remaining.ToString() +" "+ m_keyWord;
    }
}
struct PlayerActions
{
    public Quest.Action m_action;
    public string m_keyWord;
    public int m_number;
}
