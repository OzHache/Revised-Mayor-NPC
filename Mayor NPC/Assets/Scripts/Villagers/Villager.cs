using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Villager : MonoBehaviour
{
    private Movement m_movement;
    private Coroutine m_movingCoroutine;
    [SerializeField] private float m_speed = 1.0f;
    [SerializeField] private float m_maxDistance = 1.5f;
    [SerializeField] protected CharacterDialogue m_characterDialogue;

    //-----------------------
    //Resource inventory management
    //-----------------------
    Dictionary<Resource.ResourceType, int> m_resourceNeeds = new Dictionary<Resource.ResourceType, int>();
    Dictionary<Resource.ResourceType, int> m_resources = new Dictionary<Resource.ResourceType, int>();
    Dictionary<Resource.ResourceType, int> m_resourceReservations = new Dictionary<Resource.ResourceType, int>();

    [TextArea(minLines: 0, maxLines: 10),
        Tooltip("Current Activity")]
    public string m_currentActivity;

    [TextArea(minLines: 0, maxLines: 10),
        Tooltip("Formated Resources")]
    public string m_resourceLog;

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
    /// Move to specified Game Object IF I can get there
    /// </summary>
    /// <param name="target"> Destination</param>
    internal Vector3 Move(GameObject target)
    {
        return Move(target.transform.position);
    }
    internal Vector3 Move(Vector3 target)
    {
        m_currentActivity += string.Format("\nTrying to move to {0}", target.ToString());
        Vector2 destination = target;
        if (m_movement.CanGetToDestination(destination, m_maxDistance))
        {
            //stop movement
            if (m_movingCoroutine != null)
            {
                StopCoroutine(m_movingCoroutine);
                m_currentActivity += "\n stopped moving";
            }
            m_movingCoroutine = StartCoroutine(MoveTo(destination));
        }
        else
        {
            m_currentActivity += string.Format("\nFailed to find a path to {0}", target.ToString());
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
            m_currentActivity += string.Format("\nMoving to {0}",next.ToString());
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
        m_currentActivity += string.Format("\nArrived at destination {0}", destination.ToString());
    }

    //clear all the current needed resource items
    internal void ClearPendingResourceNeeds()
    {
        m_resourceNeeds.Clear();
    }

    /// <summary>
    /// remove this amount from my reservations
    /// </summary>
    /// <param name="m_resourceType">type I want to remove</param>
    /// <param name="m_amount">amount to remove</param>
    internal void ClearReservationFor(Resource.ResourceType resourceType, int amount)
    {
        //try to get reservations for this type
        if (m_resourceReservations.TryGetValue(resourceType, out int amountReserved))
        {
            //if I have reservations try to lower the amount reserved
            var remainingReservation = amountReserved - amount;
            //if there is an amount reamining in reserve, return the reservation
            if (remainingReservation > 0)
            {

                m_resourceReservations[resourceType] = remainingReservation;
            }
            //Otherwise, clear the reservation
            else
            {
                m_resourceReservations.Remove(resourceType);
            }
            GenerateResourceLog();
        }
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
            //remove this resource
            m_resources.Remove(resourceToGet);
            m_currentActivity += string.Format("\nRemoved {0} of the resource {1}", amount, resourceToGet);
            m_resourceReservations.TryGetValue(resourceToGet, out int reserved);
            var reservationsRemaining = reserved - amount;
            //see if there is a reservation remaining
            if (reservationsRemaining > 0)
            {
                m_currentActivity += string.Format("\nThere is a reservation remaining for {0} of the type {1}", reservationsRemaining, resourceToGet);
                m_resourceReservations[resourceToGet] = reservationsRemaining;
            }
            //otherwise clear the reservation
            else
            {
                m_resourceReservations.Remove(resourceToGet);
            }
            GenerateResourceLog();
            return amount;
        }
        GenerateResourceLog();
        return 0;
    }

    internal bool CheckForResourceOnHand(Resource.ResourceType type, int amount)
    {
        //return if we have this resource in the amount
        return m_resources.TryGetValue(type, out int onHand) ? onHand >= amount : false;

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
        GenerateResourceLog();
    }
    /// <summary>
    /// Not for production
    /// this will generate the resource log so the developer is aware of how much of each resource is on hand
    /// </summary>
    private void GenerateResourceLog()
    {

        m_resourceLog = string.Empty;
        //On Hand
        m_resourceLog += "On Hand\n";
        if (m_resources.Count == 0)
        {
            m_resourceLog += "---None---\n";
        }
        else
        {
            foreach (var item in m_resources)
            {
                m_resourceLog += string.Format("{0} : {1}\n", item.Key, item.Value);
            }
        }
        //Reservations
        m_resourceLog += "Reservations\n";
        if (m_resources.Count == 0)
        {
            m_resourceLog += "---None---\n";
        }
        else
        {
            foreach (var item in m_resourceReservations)
            {
                m_resourceLog += string.Format("{0} : {1}\n", item.Key, item.Value);
            }
        }
        //Needs
        m_resourceLog += "Needs\n";
        if (m_resources.Count == 0)
        {
            m_resourceLog += "---None---\n";
        }
        else
        {
            foreach (var item in m_resourceNeeds)
            {
                m_resourceLog += string.Format("{0} : {1}\n", item.Key, item.Value);
            }
        }

    }
}
