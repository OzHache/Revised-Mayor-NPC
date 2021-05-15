using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveNPCDialogue : MonoBehaviour
{
    [SerializeField]private CharacterDialogue m_dialogue;

    private void Start()
    {
        if(m_dialogue == null)
        {
            Debug.LogError("There is no Character Dialogue on this gameObject " + gameObject.name);
            Destroy(this);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            m_dialogue.Activate(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            m_dialogue.Deactivate();
        }
    }

}
