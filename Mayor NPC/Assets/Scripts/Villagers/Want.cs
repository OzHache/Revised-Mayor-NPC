using System;
using System.Collections;
using UnityEngine;

public enum LevelOfDetailLogging { k_none, k_simple, k_verbose }
public class Want : MonoBehaviour
{
    //The raw value for how much we need this want
    private float m_need = 0.0f;
    
    [SerializeField, 
        Tooltip("This is the thing that we have a want for")] 
    private Desireable m_desireable;
    public Resource.ResourceType GetDesireable() => m_desireable.m_deisredResource;
    public int GetDesiredAmount() => m_desireable.m_desiredAmount;

    #region InspectorLogging
    //----------------------------------------
    [Header("Current condition")]
    //----------------------------------------
    [SerializeField, TextArea(0, 6),
        Tooltip("current Condition of the want")]
    private string m_currentCondition;

   

    [SerializeField, Tooltip("Level of detail: \n" +
        "-none = none \n" +
        "-simple = need level & calculated need\n" +
        "-verbose = all details")]
    private LevelOfDetailLogging m_detailLevel;

    #endregion

    //name of the want
    [SerializeField,
        Tooltip("Name of the want in game", order = 3)]
    private string m_name;
    //Getter for the name
    public string GetName(){ return m_name; }

    #region CycleFields
    [Header("Cycles")]
    [SerializeField,
        Tooltip("Static Needs have a default value that needs to be met and when it is met they will no longer consider this need.\n" +
        "DEBUG CHANGEABLE.\n" +
        "CHANGES DO NOT PERSIST BUT CAN BE DYNAMICLY CHANGED HERE FOR TESTING", order = 1)]
    private bool m_staticNeed;

    [SerializeField, Delayed,
        Tooltip("Seconds for each cycle")]
    private float m_secondsPerCycle;

    [SerializeField, Range(0.0f, 10.0f), Delayed,
        Tooltip("How much need is added per cycle")]
    private float m_needIncreasePerCycle;
    //Coroutine refrence for the cycle
    private Coroutine m_cycle;

    #endregion

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

    //CRITICAL
    [SerializeField, Delayed,
         Tooltip("Point at which the need will be considered Critical")]
    private float m_criticalLevel;
    [SerializeField, Range(0.0f, 1.0f), Delayed,
         Tooltip("Additional percent when considering this an Critical Need")]
    private float m_criticalNeed;

    #endregion

    #region CalculationFields
    //----------------------------------------
    [Space(2)]
    [Header("Need calculations")]
    //----------------------------------------
    [SerializeField, Range(0.0f, 1.0f), Delayed,
        Tooltip("Weight considered when calculating preference for need")]
    private float m_weight;
    //Calculated Need
    public float m_calculatedNeed { get; private set; }


    //Interpreted Need Level
    private enum NeedLevel { k_normal, k_urgent, k_critical }

    /// <summary>
    /// Calculated field to determine the current Need Level
    /// </summary>
    private NeedLevel m_needLevel
    {
        get
        {
            switch (m_need)
            {                
                case float n when m_need >= m_urgentLevel && m_need < m_criticalLevel:  return NeedLevel.k_urgent;      //return urgent
                case float n when m_need >= m_criticalLevel:                            return NeedLevel.k_critical;    //return critical
                default:                                                                return NeedLevel.k_normal;      //return normal
            }
        }
    }
    #endregion

    #region UnityMessages
    private void Start()
    {
        GetComponent<NPCBehaviour>().AddWant(this);
        StartCoroutine(DebugCheckChange());
        m_cycle = StartCoroutine(WantCycle());
    }
    private void Update()
    {
        UpdateStatus();
    }

    #endregion

    #region PublicFunctions
    /// <summary>
    /// Initialize will populate the values based on the Data Pack and set call back to this Task
    /// </summary>
    /// <param name="m_Data"></param>
    /// <param name="wealth"></param>
    internal void Initialize(WantData m_Data, ITasks task)
    {
        throw new NotImplementedException();
    }


    /// <summary>
    /// satisfy a want by amount
    /// </summary>
    /// <param name="amount">amount to satisfy need. Will drop the overall need value </param>
    public void SatisfyBy(int amount)
    {
        //clamp the need to no lower than 0
        m_need = Mathf.Clamp(m_need -= amount, 0.0f, float.MaxValue);
        CalculateNeed();
    }

    #endregion


    #region PrivateFunction

    /// <summary>
    /// calculates the need based on the values that have been tuned
    /// Is Automatically called on each new cycle or when a the need is being satisfied
    /// </summary>
    /// <returns></returns>
    private void CalculateNeed()
    {
        //normal need is need  *  1 + weight
        float calcNeed = m_need * (1 + m_weight);
        //Urgent need is m_need * 1 + urgentNeed
        if (m_needLevel >= NeedLevel.k_urgent)
            calcNeed += m_need * m_urgentNeed;
        //critical need is (m_need + urgent) * 1 * cricitalNeed
        if (m_needLevel == NeedLevel.k_critical)
            calcNeed += m_need * m_criticalNeed;

        m_calculatedNeed = calcNeed;
    }

    /// <summary>
    /// Status updater gets called every frame and updates thier information to the inspector activity log
    /// </summary>

    private void UpdateStatus()
    {
        switch (m_detailLevel)
        {
            case LevelOfDetailLogging.k_none:                                       //nothing here
                break;      
            case LevelOfDetailLogging.k_simple:                SimpleLogging();     // used for gathering simple logging information
                break;
            case LevelOfDetailLogging.k_verbose:               VerboseLogging();    //used for gathering verbose logging information
                break;
            default:
                break;
        }
    }
    //gather all information
    private void VerboseLogging()
    {
        //initial calculations
        m_currentCondition = string.Format(
            "Need Level: {0}\n" +
            "Raw Need: {1}\n" +
            "Base weight of {2} : {3}\n"
            , m_needLevel.ToString()
            , m_need
            , m_weight
            , m_need * m_weight
            );
        //need level calculations
        if(m_needLevel > NeedLevel.k_normal)
        {
            m_currentCondition += string.Format(
                "Urgent Need of {0} Added : {1}\n", m_urgentNeed, m_urgentNeed * m_need );
        }
        if(m_needLevel > NeedLevel.k_urgent)
        {
            m_currentCondition += string.Format(
                "Critical Need of {0} Added : {1}\n", m_criticalNeed, m_criticalNeed * m_need);
        }
        //final calculated Need
        m_currentCondition += string.Format("Calculated Need: {0}\n", m_calculatedNeed);
    }

    //gather the essential information
    private void SimpleLogging()
    {
        m_currentCondition = string.Format(
            "Need Level: {0}\n" +
            "Calculated Need: {1}"
            , m_needLevel.ToString()
            , m_calculatedNeed.ToString());
    }

    /// <summary>
    /// USED ONLY FOR DEBUGGING 
    /// This section of code is not designed for product. It is supposed to allow for live edits
    /// </summary>
    private IEnumerator DebugCheckChange()
    {
        //list of changes to check
        bool staticNeed = m_staticNeed;
        float urgentLevel = m_urgentLevel;
        float criticalLevel = m_criticalLevel;

        while(true)
        {
            yield return new WaitUntil(
                () =>
                {
                    //can be extended to check for any changed values, This is not production and is intended for dubug changes ONLY
                    return
                    m_staticNeed != staticNeed ||
                    m_urgentLevel != urgentLevel ||
                    m_criticalLevel != criticalLevel;
                });
            //trigger changes that would have happened at the start of the game

            //m_static determines if this want is cycle driven or if they have a static  want level
            if (m_staticNeed != staticNeed)
            {
                if (m_cycle != null)
                {
                    StopCoroutine(m_cycle);                 //Killing this will ensure that the need will start the cycle in the correct condition
                }
                m_cycle = StartCoroutine(WantCycle());      //This will auto kill if this is a static need
                staticNeed = m_staticNeed;
            }
            //Urgent level changes, this cannot be greater than critical
            if(m_urgentLevel != urgentLevel)
            {
                //this cannot be higher than cricital 
                m_urgentLevel = Mathf.Clamp(m_urgentLevel, 0, Mathf.Clamp(m_criticalLevel - float.Epsilon, 0, float.MaxValue));
                urgentLevel = m_urgentLevel;
            }
            //Critical level changes, this cannot be less than urgent
            if (m_criticalLevel != criticalLevel)
            {
                //this cannot be higher than cricital 
                m_criticalLevel = Mathf.Clamp(m_criticalLevel, m_urgentLevel + float.Epsilon, float.MaxValue);
                criticalLevel = m_criticalLevel;
            }

        }
    }

    /// <summary>
    /// Repeats every cycle until it is told to stop 
    /// this is used to give additional NEED each cycle. 
    /// </summary>
    private IEnumerator WantCycle()
    {
        //if this is a static need cancle
        if (m_staticNeed)
        {
            yield break;
        }

        while (true)
        {
            CalculateNeed();
            yield return new WaitForSeconds(m_secondsPerCycle);
            m_need += m_needIncreasePerCycle;
        }
    }

    #endregion
}

[Serializable]
public struct Desireable
{
    public Resource.ResourceType m_deisredResource;
    public int m_desiredAmount;
}
