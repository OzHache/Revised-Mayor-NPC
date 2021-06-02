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
        Vector2 destination = target.transform.position;
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

    internal void ClearPendingResourceNeeds()
    {
        throw new NotImplementedException();
    }

    //return deficiencies if I don't have enough of this resource 
    internal int ReserveResource(Resource.ResourceType resource, int amount)
    {
        throw new NotImplementedException();
    }

    //returns true if I have enough of this resource to statisfy all reservations
    internal bool HasReservedResource(Resource.ResourceType m_resourceType)
    {
        throw new NotImplementedException();
    }

    internal bool ArrivedAtDestination(Vector3 destination)
    {
        //we have arrived and we have arrived at the destination we were looking for
        return m_movement.didArrive() && m_movement.GetDestination() == destination;
    }
}
