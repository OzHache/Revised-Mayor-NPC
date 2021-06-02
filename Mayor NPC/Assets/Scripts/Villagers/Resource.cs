using System;
using UnityEngine;

[CreateAssetMenu(fileName ="New_Resource", menuName ="Resource")]
public class Resource :ScriptableObject
{    
    public enum ResourceType { k_money, k_work, k_wood, k_stone, k_food }
    [Tooltip("Resource that will be generated")]
    public ResourceType m_generated;
    public int m_amountGenerated;
    [Tooltip("Resource that is needed in exchange")]
    public ResourceType m_needed;
    [Tooltip("Set to -1 for work and the location will determin the work value")]
    public int m_amountNeeded;

}