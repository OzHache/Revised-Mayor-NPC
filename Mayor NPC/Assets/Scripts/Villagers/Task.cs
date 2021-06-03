using System.Collections;
using UnityEngine;

public class Task : MonoBehaviour, ITasks
{
    public virtual void Satisfy(Resource.ResourceType resource) { }
    //reference to the want on the npc
    protected Want m_want;    
    //Reference to the villager that this is on
    protected Villager m_villager;
    //DataPack
    [SerializeField] protected WantData m_Data;
    /// <summary>
    /// Initialize the following protected:
    /// Villager m_Villager
    /// </summary>
    protected virtual void Initialize() 
    {
        m_villager = GetComponent<Villager>();
    }

    /// <summary>
    /// Generate a Want on this game object and initialize it with the data prefab
    /// </summary>
    protected void GenerateWant(int? amountToOverride = null)
    {
        //Generate the Want Component
        m_want = gameObject.AddComponent<Want>();
        if (amountToOverride.HasValue)
        {
            m_want.SetDesireAmount(amountToOverride.GetValueOrDefault());
        }
        //Initialize the want with Data
        m_want.Initialize(m_Data, this);
    }

}