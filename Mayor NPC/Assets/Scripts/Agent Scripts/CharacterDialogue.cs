using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDialogue : MonoBehaviour
{
    [SerializeField] protected GameObject m_dialogueObject;
    private TextMesh m_textMesh;
    [SerializeField] protected Font m_font;
    [SerializeField] protected int m_maxCharacterLenght;
    [SerializeField] protected float m_messageSpeed = 0.1f;
    [SerializeField] protected List<string> m_messages;
    protected int m_currentMessage = 0;

    protected Coroutine currentCoroutine;

    //get this from a config file
    protected Vector3 scale = new Vector3(0.319f, 0.319f, 0.319f);


    private void Reset()
    {
        if (transform.GetComponentInChildren<MeshRenderer>() == null) 
        {
            m_dialogueObject = Instantiate(new GameObject(), Vector3.zero, Quaternion.identity, gameObject.transform);
            
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
        }
        m_textMesh.font = m_font;

    }
    // Start is called before the first frame update
    void Start()
    {
        if(m_textMesh == null)
        {
            m_textMesh = m_dialogueObject.GetComponent<TextMesh>();
        }
        m_dialogueObject.SetActive(false);
    }

    internal void Activate()
    {
        m_dialogueObject.SetActive(true);
        m_textMesh.text = "";
        currentCoroutine = StartCoroutine(PrintMessage());
    }
    internal void Deactivate()
    {

        m_dialogueObject.SetActive(false);
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
        yield break;
    }
}
