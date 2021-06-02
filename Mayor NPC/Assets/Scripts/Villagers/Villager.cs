using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Villager : MonoBehaviour
{
    private Movement m_movement;
    [SerializeField] private float m_speed = 1.0f;
    [SerializeField] private float m_maxDistance = 1.5f;
    [SerializeField] protected CharacterDialogue m_characterDialogue;

    //-----------------------
    //Resource inventory management
    //-----------------------
    Dictionary<Resource.ResourceType, int> m_resourceNeeds = new Dictionary<Resource.ResourceType, int>();
    Dictionary<Resource.ResourceType, int> m_resources = new Dictionary<Resource.ResourceType, int>();
    Dictionary<Resource.ResourceType, int> m_resourceReservations = new Dictionary<Resource.ResourceType, int>();



    //villager information

    // Start is called before the first frame update
    void Start()
    {
        Setup();
    }
    //set up the character on the first load. 
    protected void Setup()
    {
        m_movement = gameObject.AddComponent<Movement>();
        //y sort
        GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(transform.position.y) * -1 + 50;
    }

    /// <summary>
    /// Called from the Villager Manager to start this villager
    /// </summary>
    internal void Activate()
    {
        //activate on activate functions

        gameObject.SetActive(true);
    }

    /// <summary>
    /// Moe to s specified Game Object IF I can get there
    /// </summary>
    /// <param name="target"> Destination</param>
    internal Vector3 Move(GameObject target)
    {
        return Move(target.transform.position);
    }
    internal Vector3 Move(Vector3 target)
    {
        Vector2 destination = target;
        if (m_movement.CanGetToDestination(destination, m_maxDistance))
        {
            StartCoroutine(MoveTo(destination));
        }
        return m_movement.GetDestination();
    }

    /// <summary>
    /// Coroutine to move to target with Transform Translate
    /// </summary>
    /// <param name="destination">Destination</param>
    private IEnumerator MoveTo(Vector2 destination)
    {

        while (!m_movement.didArrive())
        {
            Vector2 next = m_movement.GetNextCoordinate();
            while (Vector2.Distance(transform.position, next) > 0.01f)
            {
                if (GameManager.GetGameManager().isGamePaused)
                {
                    yield return null;
                }
                else
                {
                    float maxDistance = Vector2.Distance(transform.position, destination);
                    Vector2 translation = (next - (Vector2)transform.position).normalized * Mathf.Clamp(m_speed, 0, maxDistance) * Time.deltaTime;
                    transform.Translate(translation);
                    yield return null;
                }
            }
        }
    }

    //clear all the current needed resource items
    internal void ClearPendingResourceNeeds()
    {
        m_resourceNeeds.Clear();
    }

    //return deficiencies if I don't have enough of this resource 
    internal int ReserveResource(Resource.ResourceType resource, int amount)
    {
        //see how much I am currently short
        int shortAmount = 0;
        m_resourceReservations.TryGetValue(resource, out shortAmount);
        int onHand = 0;
        m_resources.TryGetValue(resource, out onHand);

        //add amount I need to increase my short by
        shortAmount += amount;
        //set  my reservation for this item the new short amount
        m_resourceReservations[resource] = shortAmount;
        //return how much I will need
        return onHand - shortAmount;
    }

    //returns true if I have enough of this resource to statisfy all reservations
    internal bool HasReservedResource(Resource.ResourceType resourceType)
    {
        //if I have reservations return if I have enough resources to satisfy reservations
        if (m_resourceReservations.TryGetValue(resourceType, out int amount))
        {
            int amountOnHand = 0;
            m_resources.TryGetValue(resourceType, out amountOnHand);
            //return if I have atleast as much as I need
            return amountOnHand >= amount;
        }
        //I have no reservations for this resource type
        return true;
    }

    internal bool ArrivedAtDestination(Vector3 destination)
    {
        //we have arrived and we have arrived at the destination we were looking for
        return m_movement.didArrive() && m_movement.GetDestination() == destination;
    }
    //returns the amount in reserve for this item
    internal int GetReservationsFor(Resource.ResourceType resourceType)
    {
        if (m_resourceReservations.TryGetValue(resourceType, out int amount))
        {
            return amount;
        }
        return 0;
    }
    /// <summary>
    /// Returns the full amount of a resource clearing the space. 
    /// THIS MUST BE RELOADED
    /// </summary>
    /// <param name="resourceToGet"> Resource to get</param>
    /// <returns>int amount of resource </returns>
    internal int GetAllOfResource(Resource.ResourceType resourceToGet)
    {
        if(m_resources.TryGetValue(resourceToGet, out int amount))
        {
            return amount;
        }
        return 0;
    }

    //Add resources back to inventory
    internal void AddResource(Resource.ResourceType resourceType, int amountReturned)
    {
        //if there is an amount already in the resource add that to the amount returned
        if (m_resources.TryGetValue(resourceType, out int amount))
        {
            amountReturned += amount;
        }
        m_resources[resourceType] = amountReturned;
    }
}
