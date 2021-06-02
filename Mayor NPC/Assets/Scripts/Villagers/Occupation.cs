using UnityEngine;
[CreateAssetMenu(fileName = "Occupation", menuName = "NPC_Occupation")]

/* Occupations are scriptable object that designate
 *
 *
 */
public class Occupation : ScriptableObject
{
    [Tooltip("Can be any location that the job requires")]
    [SerializeField] private Transform m_location;

    [Tooltip("How long the job should last")]
    [Range(0, 12)]
    [SerializeField] private float m_hours;
    [SerializeField,
        Tooltip("Resource Generated at this location for the worker")]
    private Resource m_workerResource;
    [SerializeField,
       Tooltip("Resource Generated at this location for the customer")]
    private Resource m_customerResource;


}
