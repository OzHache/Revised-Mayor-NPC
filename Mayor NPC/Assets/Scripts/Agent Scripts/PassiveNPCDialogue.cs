using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveNPCDialogue : MonoBehaviour
{
    private CharacterDialogue dialogue;

    private void Start()
    {
        dialogue = GetComponent<CharacterDialogue>();
        if(dialogue == null)
        {
            Debug.LogError("There is no Character Dialogue on this gameObject " + gameObject.name);
            Destroy(this);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            dialogue.Activate();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            dialogue.Deactivate();
        }
    }

}
