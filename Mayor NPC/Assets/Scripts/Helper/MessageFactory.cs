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
        if (s_instance != null)
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
        GameObject parent = Instantiate(new GameObject(), messanger.transform.position, Quaternion.identity);
        GameObject messageObject = Instantiate(FloatingMessageObject, Vector3.zero, Quaternion.identity, parent.transform);
        messageObject.transform.position = Vector3.zero;
        messageObject.name = " message Fact";
        FloatingMessage script = messageObject.GetComponent<FloatingMessage>();
        if (script == null)
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
