using System;
using System.Collections;
using UnityEngine;

//Wealth Generates a want on start
public class Wealth : MonoBehaviour, ITasks
{
    private Want m_want;
    [SerializeField] private WantData m_Data;
    public void Satisfy(Resource.ResourceType resource, int amount)
    {
        throw new System.NotImplementedException();
    }

    // Use this for initialization
    void Start()
    {
        GenerateWant();
    }

    private void GenerateWant()
    {
        //Generate the Want Component
        m_want = gameObject.AddComponent<Want>();
        //Initialize the want with Data
        m_want.Initialize(m_Data, this);
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}