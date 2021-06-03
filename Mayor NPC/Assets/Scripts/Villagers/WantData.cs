using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWantData", menuName = "WantDataPack"), Serializable]
public class WantData : ScriptableObject
{
    //Data
    //Desireable 
    [SerializeField,
        Tooltip("This is the thing that we have a want for")]
    private Desireable m_desireable;
    public Resource.ResourceType GetDesireable() => m_desireable.m_deisredResource;
    public int GetDesiredAmount() => m_desireable.m_desiredAmount;

    //name of the want
    [SerializeField,
        Tooltip("Name of the want in game", order = 3)]
    private string m_name;
    public string GetName() => m_name;

    //Cycles
    [Header("Cycles")]
    [SerializeField,
      Tooltip("Static Needs have a default value that needs to be met and when it is met they will no longer consider this need.\n" +
      "DEBUG CHANGEABLE.\n" +
      "CHANGES DO NOT PERSIST BUT CAN BE DYNAMICLY CHANGED HERE FOR TESTING", order = 1)]
    private bool m_staticNeed;
    public bool IsStatic() => m_staticNeed;
    public void SetStatic(bool value) { m_staticNeed = value; }

    //Cycle
    [SerializeField, Delayed,
        Tooltip("Seconds for each cycle")]
    private float m_secondsPerCycle;
    public float GetCycleDelay() => m_secondsPerCycle;

    [SerializeField, Range(0.0f, 10.0f), Delayed,
        Tooltip("How much need is added per cycle")]
    private float m_needIncreasePerCycle;
    public float GetIncreasePerCycle() => m_needIncreasePerCycle;

    internal void SetAmountDesired(int amount)
    {
        throw new NotImplementedException();
    }


    #region NeedLevels
    /// <summary>
    /// Three levels of need: normal, urgent, critical
    /// </summary>

    //URGENT 
    [SerializeField, Delayed,
         Tooltip("Point at which the need will be considered Urgent")]
    private float m_urgentLevel;
    
    [SerializeField, Range(0.0f, 1.0f), Delayed,
         Tooltip("Additional percent when considering this an Urgent Need")]
    private float m_urgentNeed;
    //Getters and setters
    public float GetUrgentNeed() => m_urgentNeed;
    public float GetUrgentLevel() => m_urgentLevel; 
    public void SetUrgentLevel(float value) { m_urgentLevel = value; }

    //CRITICAL
    [SerializeField, Delayed,
         Tooltip("Point at which the need will be considered Critical")]
    private float m_criticalLevel;
    [SerializeField, Range(0.0f, 1.0f), Delayed,
         Tooltip("Additional percent when considering this an Critical Need")]
    private float m_criticalNeed;
    //Getters and setters
    public float GetCriticalNeed() => m_criticalNeed;
    public float GetCriticalLevel() => m_criticalLevel;
    public void SetCriticalLevel(float value) { m_criticalLevel = value; }

    #endregion
    //CalculatedNeed
    //----------------------------------------
    [Space(2)]
    [Header("Need calculations")]
    //----------------------------------------
    [SerializeField, Range(0.0f, 1.0f), Delayed,
        Tooltip("Weight considered when calculating preference for need")]
    private float m_weight;
    public float GetWeight() => m_weight;


    internal int GetAmountDesired() => m_desireable.m_desiredAmount;
}