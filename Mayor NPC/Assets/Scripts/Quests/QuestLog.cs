using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class QuestLog : MonoBehaviour
{
    private TextMeshProUGUI m_text;
    private Quest m_quest;

    public Quest GetQuest(){ return m_quest; }
    private void Awake()
    {
        m_text = GetComponent<TextMeshProUGUI>();
    }

    public void SetQuest(Quest quest)
    {
        m_quest = quest;
        UpdateUI();
    }

    internal string GetQuestKey()
    {
        return m_quest.GetKey();
    }

    internal int UpdateQuest(Quest.ActionType action, int amount)
    {
        int left = m_quest.UpdateQuest(action, amount);
        
        UpdateUI();
        return left;
    }

    private void UpdateUI()
    {
        m_text.text = m_quest.GetQuest();
    }
    public void CloseQuest()
    {
        Debug.Log("Quest Complete" + m_quest.name);
        Destroy(gameObject);
    }
}

