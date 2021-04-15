using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterDialogue : MonoBehaviour
{
    [SerializeField]private RectTransform m_choiceRect;
    private List<GameObject> m_choices = new List<GameObject>();

    private TextMesh m_textMesh;
    private MeshRenderer m_meshRenderer;
    [SerializeField] protected Font m_font;
    [SerializeField] protected int m_maxCharacterLenght;
    [SerializeField] protected float m_messageSpeed = 0.1f;
    [SerializeField] protected List<string> m_messages;
    protected int m_currentMessage = 0;

    //Dialogue management
    [SerializeField]private string m_dialogueOwnerId;
    private DialogueContainer m_dialogueContainer;
    private string m_branchID = "";
    private string m_choiceId = "";

    protected Coroutine currentCoroutine;

    //get this from a config file
    protected Vector3 scale = new Vector3(0.319f, 0.319f, 0.319f);


    private void Reset()
    {
        /*if (transform.GetComponentInChildren<MeshRenderer>() == null) 
        {
            *//*m_dialogueObject = Instantiate(new GameObject(), Vector3.zero, Quaternion.identity, gameObject.transform);*//*
            
            var meshRenderer = m_dialogueObject.AddComponent<MeshRenderer>();
            m_textMesh = m_dialogueObject.AddComponent<TextMesh>();
            //Set up the Text
            m_textMesh.font = m_font;
            m_textMesh.text = gameObject.name;
            m_textMesh.anchor = TextAnchor.MiddleCenter;
            //Set up the Dialogue Game Object
            m_dialogueObject.transform.localScale = scale;
            //Set up the Mesh Renderer
            meshRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            meshRenderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
            meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            meshRenderer.receiveShadows = false;
            m_dialogueObject.transform.position = Vector3.up;
        }*/
        m_textMesh.font = m_font;

    }
    // Start is called before the first frame update
    void Start()
    {
        m_textMesh = GetComponent<TextMesh>();
        m_meshRenderer = GetComponent<MeshRenderer>();
        if(m_meshRenderer != null)
            m_meshRenderer.enabled = false;
        if(m_choiceRect == null)
        {
            Debug.LogError("This needs a choices Panel set up" + gameObject.name);
        }
    }

    internal void Activate()
    {
        //see if I currently have a dialogue
        if(m_dialogueContainer == null)
        {
            //Load the dialogue
            LoadDialogue();
        }
        if (m_meshRenderer != null)
            m_meshRenderer.enabled = true;
        m_textMesh.text = "";
        currentCoroutine = StartCoroutine(PrintMessage());
    }

    private void LoadDialogue()
    {
        if (m_dialogueContainer == null)
        {
            m_dialogueContainer = DialogueLoader.GetLoader().GetDialogueContainerFor(m_dialogueOwnerId);
        }
        if(m_dialogueContainer == null)
        {
            Debug.Log("There is no Dialogue for Id" + m_dialogueOwnerId);
            return;
        }
        //set the initial values
        m_branchID = m_dialogueContainer.m_dialogueBranches[0].m_id;
        //m_choice Id is set when there is a choice made
        DialogueBranch branch;
        //if we succeed here
        if(m_dialogueContainer.GetNextBranch(out branch, m_choiceId))
        {
            m_currentMessage = 0;
            m_messages[0] = branch.m_maintext;
            //clear the choices
            ClearChoices();

            for(var i = 0; i < branch.m_choices.Count; i++)
            {
                //see if i > than the amount of children in choices, then duplicate the last one
                GameObject _choice;
                if (i > m_choiceRect.childCount-1)
                {
                    _choice = Instantiate<GameObject>(m_choiceRect.GetChild(0).gameObject, m_choiceRect);
                }
                else
                {
                    _choice = m_choiceRect.GetChild(i).gameObject;
                }
                _choice.GetComponentInChildren<TextMeshProUGUI>().text = branch.m_choices[i].m_choiceText;
                //set the name of the choice to the choice id so we can grab it later
                _choice.name = branch.m_choices[i].m_choiceId;
                _choice.SetActive(false);
                m_choices.Add(_choice);
                    
                
            }
        }
    }

    private void ClearChoices()
    {
        //Deactivate all the choices
        m_choices.Clear();
        for (var i = m_choiceRect.childCount -1; i >= 0; i--)
        {
            m_choiceRect.GetChild(i).gameObject.SetActive(false);
            
        }
    }
    public void OnButtonClick(GameObject gameObject)
    {
        //get the name 
        m_choiceId = gameObject.name;
        LoadDialogue();
        Activate();
    }

    internal void Deactivate()
    {
        if (m_meshRenderer != null)
            m_meshRenderer.enabled = false;
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
    }
    internal void SetCurrentMessage(string message, bool clear = false)
    {
        Deactivate();
        if (clear)
        {
            m_messages.Clear();
        }
        m_textMesh.text = "";
        m_messages.Add(message);
        m_currentMessage = m_messages.IndexOf(message);
        Activate();
    }

    protected IEnumerator PrintMessage()
    {
        m_textMesh.text = new string(' ', m_maxCharacterLenght) + "\n";
        int currentCharacter = 0;
        int charactersOnLine = 0;
        string currentMessage = m_messages[m_currentMessage];
        currentMessage += " ";
        //fill the text block with a blank array of approximately how many lines we will need
        


        while (currentCharacter < currentMessage.Length)
        {
            //Determine if this word needs to be on a new line
            if (currentMessage.Substring(currentCharacter, currentMessage.IndexOf(" ", currentCharacter) - currentCharacter).Length + charactersOnLine > m_maxCharacterLenght)
            {
                charactersOnLine = 0;
                m_textMesh.text += "\n";
                if (currentMessage.Substring(currentCharacter, currentMessage.IndexOf(" ", currentCharacter)- currentCharacter).Length > m_maxCharacterLenght)
                {
                    while (currentMessage.IndexOf(" ", currentCharacter) > 1)
                    {
                        m_textMesh.text += currentMessage.Substring(currentCharacter, 1);
                        currentCharacter++;
                        yield return new WaitForSeconds(m_messageSpeed);
                    }
                    charactersOnLine = 0;
                    m_textMesh.text += "\n";
                }
            }
            else
            {
                m_textMesh.text += currentMessage.Substring(currentCharacter, 1);
                currentCharacter++;
                charactersOnLine++;
            }
            yield return new WaitForSeconds(m_messageSpeed);
        }
        currentCoroutine = null;
        
        ActivateChoices();
    }

    private void ActivateChoices()
    {
        foreach(var choice in m_choices)
        {
            choice.SetActive(true);
        }
    }
}
