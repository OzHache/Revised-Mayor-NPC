using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageFactory : MonoBehaviour
{
    
    public enum MessageType
    {
        k_FloatingMessage,
        k_Ununsed

    }

    [SerializeField] private GameObject FloatingMessageObject;
    private static MessageFactory s_instance;
    
    
    private void Awake()
    {
       if(s_instance != null)
        {
            Debug.LogError("There can be only one message factory: " + gameObject.name);
            Destroy(this);
        }
        else
        {
            s_instance = this;
        }
    }
    public static MessageFactory GetMessageFactory() 
    {
        return s_instance;
    }

    public void CreateFloatingMessage(string message, FloatingMessage.MessageCategory messageType, GameObject messanger)
    {
        var messageObject = Instantiate(FloatingMessageObject, Vector3.zero, Quaternion.identity,messanger.transform);
        messageObject.transform.position = Vector3.zero;
        messageObject.name = " message Fact";
        var script = messageObject.GetComponent<FloatingMessage>();
        if(script == null)
        {
            Debug.LogError("This prefab does not have the expected monobehaviour: " + messageObject.name);
            Destroy(messageObject);
        }
        else
        {
            script.SetMessage(message, messageType, messanger);
        }
        
        
    }
}
