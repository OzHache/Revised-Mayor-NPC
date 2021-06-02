using System;
using UnityEngine;

public class Location : MonoBehaviour
{
    //location
    [SerializeField] private Transform m_location;
    public Vector3 GetTransactionLocation() { return m_location.position; }
    //Resource that this location generates
    [SerializeField] private Resource m_generates;
    /// <summary>
    /// Get the resouce that this location generates
    /// </summary>
    /// <returns>resource generated at this location</returns>
    public Resource.ResourceType GetResource() { return m_generates.m_generated; }
    public int GetResourceRequiredPerTransaction() { return m_generates.m_amountNeeded; }
    public int GetResourceGeneratedPerTransaction() { return m_generates.m_amountGenerated; }
    public Resource.ResourceType GetResourceNeeded() { return m_generates.m_needed; }
    /// <summary>
    /// Get the amount you need in order to return the amount of the needed resource
    /// </summary>
    /// <param name="amount">Amount you want</param>
    /// <returns>Amount you will need to pay</returns>
    public int GetAmountNeededToFulfill(int amount) 
    {
        //lowest amount of transactions to fulfill
        int transactions = Mathf.CeilToInt((float)amount / (float)m_generates.m_amountGenerated);
        //Amount Needed 
        int amountNeeded = transactions * m_generates.m_amountNeeded;
        return amountNeeded; 
    }

    /// <summary>
    /// attempt to transact for the amount wanted
    /// </summary>
    /// <param name="amountWanted">Amount of generated resource seeking</param>
    /// <param name="inAmount">Reference to the amount of resources had</param>
    /// <param name="amount"> outs the amount returned</param>
    /// <returns>true if the transaction succeeded</returns>
    public bool Transact(int amountWanted, ref int inAmount, out int amount)
    {
        amount = 0;

        //determine how many transactions are required
        var transactions = Mathf.CeilToInt((float)amountWanted / (float)m_generates.m_amountGenerated);

        //if the amount passed in is at lease equal to the amount needed modify the in amount and return the return amount
        var neededAmount = m_generates.m_amountNeeded * transactions;
        //Determine if the  user has enough items for this transactions
        if(inAmount >= neededAmount)
        {
            inAmount -= m_generates.m_amountNeeded;
            amount = m_generates.m_amountGenerated;
            return true;
        }
        return false;

    }
}