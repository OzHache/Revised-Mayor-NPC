using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FloatingMessage : MonoBehaviour
{
    public enum MessageCategory { k_Stamina, k_HP}
    [SerializeField] static Color s_HpColor = Color.red;
    [SerializeField] static Color s_StaminaColor = Color.green;
    [SerializeField] private TextMeshProUGUI textMesh;
    private GameObject m_parent;
    
    private void Awake()
    {
        
    }
    public void SetMessage(string message, MessageCategory type, GameObject messanger)
    {
        //Set the message
        textMesh.text = message;
        switch (type)
        {
            case MessageCategory.k_HP:
                textMesh.color = s_HpColor;
                break;
            case MessageCategory.k_Stamina:
                textMesh.color = s_StaminaColor;
                break;
            default:
                break;
        }
        /*transform.parent = null;
        m_parent = Instantiate(new GameObject(), messanger.transform.position, Quaternion.identity);
        m_parent.name = "NewParent";
        transform.parent = m_parent.transform;*/
        
    }

    //called from the animation
    private void End()
    {
        
        Destroy(gameObject.transform.parent.gameObject);
    }

}
