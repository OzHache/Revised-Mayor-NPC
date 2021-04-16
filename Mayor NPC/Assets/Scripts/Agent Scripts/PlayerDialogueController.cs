using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
class PlayerDialogueController : MonoBehaviour
{

    [SerializeField]private CharacterDialogue m_dialogue;
    private Coroutine m_talking;
    [SerializeField] private float m_talkDuration = 5.0f;
    private List<ToolType> m_toolQuests = new List<ToolType>();

    private void Start()
    {
    }

    internal void TalkAbout(UIInteractable item, Action questAction = null)
    {
        if (m_talking != null)
            StopCoroutine(m_talking);

        string message = "That is a " + item.GetDescription();
        if (!GameManager.GetGameManager().GetTools().Contains(item.GetTool()))
        {
            if (!(item.GetTool() == ToolType.None))
            {
                message += ". It will require a " + item.GetTool().ToString();
                //see if there is not already a quest for this tool
                if (!m_toolQuests.Contains(item.GetTool()))
                {
                    //add Quests
                    Quest quest = new Quest(Quest.ActionType.Craft, item.GetTool().ToString(), 1, questAction);
                    QuestManager.GetQuestManager().AddQuest(quest);
                    //add this tool to the list of quests.
                    m_toolQuests.Add(item.GetTool());
                }
                if (m_talking != null)
                    StopCoroutine(m_talking);
            }
        
        }
        //manage all the other descriptions

        m_dialogue.SetCurrentMessage(message, true);

        m_talking = StartCoroutine(Talking());
    }
    private IEnumerator Talking()
    {
        bool running = true;
        float timer = 0.0f;
        while (running)
        {
            //other interactions
            timer += Time.deltaTime;
            if (timer > m_talkDuration)
                running = false;
            yield return null;
        }
        m_dialogue.Deactivate();
        StopCoroutine(Talking());
        yield break;
    }

    internal void Say(string message)
    {
        //stop talking
        if (m_talking != null)
            StopCoroutine(m_talking);

        m_dialogue.SetCurrentMessage(message, true);

        m_talking = StartCoroutine(Talking());

    }
}

