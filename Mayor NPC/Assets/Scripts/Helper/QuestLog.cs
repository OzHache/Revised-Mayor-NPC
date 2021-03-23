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

    public void SetQuest(Quest quest)
    {
        m_quest = quest;
        UpdateUI();
    }

    internal string GetQuestKey()
    {
        return m_quest.GetKey();
    }

    internal void UpdateQuest(Quest.Action action)
    {
        m_quest.UpdateQuest(action);
        if (m_quest.IsCompleted())
        {
            QuestManager.GetQuestManager().RemoveQuest(this);
        }
        UpdateUI();
    }

    private void UpdateUI()
    {
        m_text.text = m_quest.GetQuest();
    }
}

