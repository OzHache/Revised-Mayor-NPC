using System;
using System.Collections;
using UnityEngine;
/// <summary>
/// Weath generates a want on start
/// This will continue to be a want until it is satisfied. 
/// Once satisfied the Want will be destroyed
/// if the reference to the want is null then weath will continue to check on the weath of the villager
/// If the weath drops below a specific level then a want will be generated again
/// </summary>
public class Wealth : Task
{
    [SerializeField,
        Tooltip("How often to check for this")]
    private float m_checkDelay;
    
    //resource we want
    private const Resource.ResourceType m_resourceType = Resource.ResourceType.k_money;

    private int m_amount;
    
    // Use this for initialization
    private void Start()
    {
        Initialize();
    }

    /// <summary>
    /// Should be called when the resource we want has been satisfied
    /// </summary>
    /// <param name="resource">What we want</param>
    /// <param name="amount">amount we want</param>
    public override void  Satisfy(Resource.ResourceType resource)
    {
        if(resource != m_resourceType)
        {
            Debug.LogError("This should not be triggering here with this resource.");
        }
        //This satisfies our need
        if(m_villager.CheckForResourceOnHand(m_resourceType, m_amount))
        {
            m_villager.ClearReservationFor(m_resourceType, m_amount);
            //tell this object to destroy itself
            m_want.SetToDestruct();
            m_want = null;
            //Start the wealth monitor
            StartCoroutine(MonitorWealth());
        }
        else
        {
            //this should not happen but I want to know if it does
            Debug.LogError(string.Format("Error occured when attempting to satisfy {0} Expected {1}", m_resourceType, m_amount));
        }

    }

    /// <summary>
    /// Monitor the wealth of the player
    /// </summary>
    private IEnumerator MonitorWealth()
    {
        while (true)
        {
            //determine if we have enough of the desired wealth
            if(m_villager.CheckForResourceOnHand(m_resourceType, m_amount))
            {
                yield return new WaitForSeconds(m_checkDelay);
            }
            //otherwise generate a want
            else
            {
                GenerateWant();
                break;
            }
        }
    }

    protected override void Initialize()
    {
        base.Initialize();
        //Get the amount from the data
        m_amount = m_Data.GetAmountDesired();
        //Start the weath monitor
        StartCoroutine(MonitorWealth());
    }
}