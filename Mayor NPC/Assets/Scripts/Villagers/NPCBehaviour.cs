using System;
using System.Collections.Generic;
using UnityEngine;
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

    private List<Want> m_wants = new List<Want>();



    private void Start()
    {
        //Check for existing wants
        m_wants.AddRange(GetComponents<Want>());
        

    }

    //called from start on wants when a new want is added to the player
    internal void AddWant(Want want)
    {
        if(!m_wants.Contains(want))
            m_wants.Add(want);
    }
}
