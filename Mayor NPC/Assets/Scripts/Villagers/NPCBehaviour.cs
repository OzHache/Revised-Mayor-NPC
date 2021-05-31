using UnityEngine;
using System.Collections.Generic;
/// <summary>
/// NPCs 
/// </summary>
public class NPCBehaviour : MonoBehaviour
{
    ///

    [TextArea]
    [Tooltip("What am I doing right now")]
    [InspectorName("Activity")]
    public string m_activity;

    [Tooltip("Assigned Job, can be hard coded or they will look for an open job if they need money")]
    [SerializeField] private Occupation m_occupation;

    [Tooltip("List of wants, they are added to the NPC as Components")]
    [SerializeField] private List<Want> m_wants;



    private void Start()
    {
    }
}
