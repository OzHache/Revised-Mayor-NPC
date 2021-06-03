using System.Collections;
using UnityEngine;

/// <summary>
/// Hunger generates a want on start
/// A Food Want will have a building need
/// Hunger is never satisfied 
/// </summary>
public class Hunger : Task
{
    //resource we want
    private const Resource.ResourceType m_foodType = Resource.ResourceType.k_food;
    //How much food we need
    private int m_foodNeeded = 0;
    //Calculated hunger Level
    private int m_hungerLevel;
        //Current Starvation Level
    private float m_currentStarvationLevel;

    [TextArea(minLines: 0, maxLines: 5),
        Tooltip("Current Activity on Hunger Task")]
    public string m_activity;
    [SerializeField] private bool m_recordHistory = false;

    [SerializeField, Range(0, 10),
        Tooltip("Amount needed to satisfy a level of hunger")] 
    private int m_foodPerHungerLevel;

    [SerializeField, Range(0f, 10f),
         Tooltip("a new hunger level will be added when the current starvation reaches this number")]
    private float m_hungerAddAt;
    
    [SerializeField, Range(0f, 1f),
        Tooltip("Amount to increase starvation per cycle")]
    private float m_starvationPerCycle;

    [SerializeField, Range (0f, 10f),
        Tooltip("Cycle lenght in seconds to increase starvation level")]
    private float m_hungerCycleLenght;


    public override void Satisfy(Resource.ResourceType resource)
    {
        if(resource == m_foodType)
        {
            //see if I have enough food to satisfy my hunger
            var food = m_villager.GetAllOfResource(m_foodType);
            if(food >= m_foodNeeded)
            {
                //consume the food
                food -= m_foodNeeded;
                m_foodNeeded = 0;
                m_hungerLevel = 0;
                m_villager.AddResource(m_foodType, food);
                m_want.SetToDestruct();
            }
            //otherwise consume what we have and generate another want
            else
            {

                m_foodNeeded -= food;
                CalculateHungerLevel();
                m_want.SetToDestruct();
                //generate want and override need
                GenerateWant(m_foodNeeded);
            }
        }
    }

    //calculate the hunger level when I have consumed food
    private void CalculateHungerLevel()
    {
        m_hungerLevel = m_foodPerHungerLevel / m_foodNeeded;
    }

    // Use this for initialization
    void Start()
    {
        Initialize();
    }

    protected override void Initialize()
    {
        base.Initialize();
        StartCoroutine(HungerCycle());
    }

    //Build hunger and once it becomes greater than a certian amount generate a want to food. 
    private IEnumerator HungerCycle()
    {
        while (true)
        {

            yield return new WaitForSeconds(m_hungerCycleLenght);
            //if we are not recording history
            if (!m_recordHistory) 
            {
                m_activity = string.Empty; 
            }
            m_currentStarvationLevel += m_starvationPerCycle;
            m_activity += string.Format("The current hunger level is {0}\n" +
                "The amount of food needed is {1}\n" +
                "current starvation is {2}\n" +
                "Next Hunger level at {3}"
                , m_hungerLevel
                , m_foodNeeded
                , m_currentStarvationLevel
                , m_hungerAddAt);
            //If we would go up a starvation level
            if(m_currentStarvationLevel >= m_hungerAddAt)
            {
                //check if we have food for this level
                int foodOnHand = m_villager.GetAllOfResource(m_foodType);
                if (foodOnHand >= m_hungerLevel + 1 * m_foodPerHungerLevel)
                {
                    //eat food needed
                    foodOnHand -= m_hungerLevel + 1 * m_foodPerHungerLevel;
                    //if there is any remaining
                    if (foodOnHand > 0)
                    {
                        m_villager.AddResource(m_foodType, foodOnHand);
                    }
                }
                else
                {
                    //add a hunger level
                    m_hungerLevel++;
                    m_foodNeeded += m_foodPerHungerLevel;
                }
                //reset the starvation level plus how much was over the amount to generate a hunger level
                m_currentStarvationLevel %= m_hungerAddAt;
                //if I do not have a want already, and I need food generate a want
                if(m_want == null && m_hungerLevel > 0)
                {
                    GenerateWant(m_foodNeeded);
                }
                

            }
        }
    }
}