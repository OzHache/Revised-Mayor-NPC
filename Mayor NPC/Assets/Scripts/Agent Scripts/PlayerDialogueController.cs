using System;
using System.Collections;
using UnityEngine;


[Serializable]
[RequireComponent(typeof(CharacterDialogue))]
class PlayerDialogueController : MonoBehaviour
{
    private CharacterDialogue m_dialogue;
    private Coroutine m_talking;
    [SerializeField] private float m_talkDuration = 5.0f;

    private void Start()
    {
        m_dialogue = GetComponent<CharacterDialogue>();
    }

    internal void TalkAbout(UIInteractable item)
    {
        if (m_talking != null)
            StopCoroutine(m_talking);

        string message = "That is a " + item.GetDescription();
        if (!GameManager.GetGameManager().GetTools().Contains(item.GetTool()))
        {
            if (!(item.GetTool() == ToolType.None))
            {
                message += ". It will require a " + item.GetTool().ToString();
                //add Quests
                PlayerActions action = new PlayerActions();
                action.m_action = Quest.Action.Craft;
                action.m_keyWord = item.GetTool().ToString();
                action.m_number = 1;
                QuestManager.GetQuestManager().CheckQuest(action);
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

