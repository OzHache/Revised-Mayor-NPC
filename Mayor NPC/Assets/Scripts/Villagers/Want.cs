using System;
using System.Collections;
using UnityEngine;

public enum LevelOfDetailLogging { k_none, k_simple, k_verbose }
public class Want : MonoBehaviour
{
    [SerializeField] WantData m_data;
    private ITasks m_task;

    //The raw value for how much we need this want
    private float m_need = 0.0f;
    //Coroutine refrence for the cycle
    private Coroutine m_cycle;

    //Calculated Need
    public float m_calculatedNeed { get; private set; }

    //Interpreted Need Level

    /// <summary>
    /// Three levels of need: normal, urgent, critical
    /// </summary>
    private enum NeedLevel { k_normal, k_urgent, k_critical }

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
    private int? m_OverrideAmountWanted;

    #endregion

    //Getters for m_data
    public Resource.ResourceType GetDesireable() => m_data.GetDesireable();
    public int GetDesiredAmount()=> m_OverrideAmountWanted ?? m_data.GetDesiredAmount();   
    public string GetName()=> m_data.GetName();

    internal void SetDesireAmount(int amount)
    {
        m_OverrideAmountWanted = amount;
    }


    #region CalculationFields

    /// <summary>
    /// Calculated field to determine the current Need Level
    /// </summary>
    private NeedLevel m_needLevel
    {
        get
        {
            switch (m_need)
            {                
                case float n when m_need >= m_data.GetUrgentLevel() && m_need < m_data.GetCriticalLevel():  
                    return NeedLevel.k_urgent;      //return urgent
                case float n when m_need >= m_data.GetCriticalLevel():                            
                    return NeedLevel.k_critical;    //return critical
                default:                                                               
                    return NeedLevel.k_normal;      //return normal
            }
        }
    }
    #endregion

    #region UnityMessages
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
    internal void Initialize(WantData data, ITasks task)
    {
        m_data = data;
        m_task = task;

        GetComponent<NPCBehaviour>().AddWant(this);
        StartCoroutine(DebugCheckChange());
        m_cycle = StartCoroutine(WantCycle());
    }


    /// <summary>
    /// satisfy a want by amount
    /// </summary>
    /// <param name="amount">amount to satisfy need. Will drop the overall need value </param>
    public void SatisfyBy(int amount, Resource.ResourceType type)
    {
        //check that this want cares about this resource type
        if (type == m_data.GetDesireable())
        {
            m_task.Satisfy(m_data.GetDesireable());
            //clamp the need to no lower than 0
            m_need = Mathf.Clamp(m_need -= amount, 0.0f, float.MaxValue);
        } 
        CalculateNeed();
    }

    #endregion


    #region PrivateFunction
    /// <summary>
    /// Destroy this component and remove it from the NPC Behaviours
    /// </summary>
    internal void SetToDestruct()
    {
        //tell the NPC behaviour to forget this 
        GetComponent<NPCBehaviour>().RemoveWant(this);
        Destroy(this, .01f);
    }

    /// <summary>
    /// calculates the need based on the values that have been tuned
    /// Is Automatically called on each new cycle or when a the need is being satisfied
    /// </summary>
    /// <returns></returns>
    private void CalculateNeed()
    {
        //normal need is need  *  1 + weight
        float calcNeed = m_need * (1 + m_data.GetWeight());
        //Urgent need is m_need * 1 + urgentNeed
        if (m_needLevel >= NeedLevel.k_urgent)
            calcNeed += m_need * m_data.GetUrgentNeed();
        //critical need is (m_need + urgent) * 1 * cricitalNeed
        if (m_needLevel == NeedLevel.k_critical)
            calcNeed += m_need * m_data.GetCriticalNeed();

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
            , m_data.GetWeight()
            , m_need * m_data.GetWeight()
            );
        //need level calculations
        if(m_needLevel > NeedLevel.k_normal)
        {
            m_currentCondition += string.Format(
                "Urgent Need of {0} Added : {1}\n", m_data.GetUrgentNeed(), m_data.GetUrgentNeed() * m_need );
        }
        if(m_needLevel > NeedLevel.k_urgent)
        {
            m_currentCondition += string.Format(
                "Critical Need of {0} Added : {1}\n", m_data.GetCriticalNeed(), m_data.GetCriticalNeed() * m_need);
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
        bool staticNeed = m_data.IsStatic();
        float urgentLevel = m_data.GetUrgentLevel();
        float criticalLevel = m_data.GetCriticalLevel();

        while(true)
        {
            yield return new WaitUntil(
                () =>
                {
                    //can be extended to check for any changed values, This is not production and is intended for dubug changes ONLY
                    return
                    m_data.IsStatic() != staticNeed ||
                    m_data.GetUrgentLevel() != urgentLevel ||
                    m_data.GetCriticalLevel() != criticalLevel;
                });
            //trigger changes that would have happened at the start of the game

            //m_static determines if this want is cycle driven or if they have a static  want level
            if (m_data.IsStatic() != staticNeed)
            {
                if (m_cycle != null)
                {
                    StopCoroutine(m_cycle);                 //Killing this will ensure that the need will start the cycle in the correct condition
                }
                m_cycle = StartCoroutine(WantCycle());      //This will auto kill if this is a static need
                staticNeed = m_data.IsStatic();
            }
            //Urgent level changes, this cannot be greater than critical
            if(m_data.GetUrgentLevel() != urgentLevel)
            {
                //this cannot be higher than cricital 
                m_data.SetUrgentLevel(Mathf.Clamp(m_data.GetUrgentLevel(), 0, Mathf.Clamp(m_data.GetCriticalLevel() - float.Epsilon, 0, float.MaxValue)));
                urgentLevel = m_data.GetUrgentLevel();
            }
            //Critical level changes, this cannot be less than urgent
            if (m_data.GetCriticalLevel() != criticalLevel)
            {
                //this cannot be higher than cricital 
                m_data.SetCriticalLevel( Mathf.Clamp(m_data.GetCriticalLevel(), m_data.GetUrgentLevel() + float.Epsilon, float.MaxValue));
                criticalLevel = m_data.GetCriticalLevel();
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
        if (m_data.IsStatic())
        {
            yield break;
        }

        while (true)
        {
            CalculateNeed();
            yield return new WaitForSeconds(m_data.GetCycleDelay());
            m_need += m_data.GetIncreasePerCycle();
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
