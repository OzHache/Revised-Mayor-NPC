using UnityEngine;
[CreateAssetMenu(fileName = "Occupation", menuName = "NPC_Occupation")]

/* Occupations are scriptable object that designate
 *
 *
 */
public class Occupation : ScriptableObject
{
    [Tooltip("Can be any location that the job requires")]
    [SerializeField] private Transform location;

    [Tooltip("How long the job should last")]
    [Range(0, 12)]
    [SerializeField] private float hours;


}
