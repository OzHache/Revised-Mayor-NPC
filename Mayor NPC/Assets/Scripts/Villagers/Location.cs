using UnityEngine;

public class Location : MonoBehaviour
{
    //location
    public Vector3 m_location { get { return transform.position; } }
    //Resource that this location generates
    [SerializeField] private Resource m_generates;
    /// <summary>
    /// Get the resouce that this location generates
    /// </summary>
    /// <returns>resource generated at this location</returns>
    public Resource.ResourceType GetResource() { return m_generates.m_generated; }
    public int GetAmountGenerated() { return m_generates.m_amountGenerated; }
    public Resource.ResourceType GetResourceNeeded() { return m_generates.m_needed; }
    public int GetAmountNeeded() { return m_generates.m_amountNeeded; }
}