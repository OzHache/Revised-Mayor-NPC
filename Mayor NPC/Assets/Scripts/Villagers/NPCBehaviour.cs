using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// NPCs 
/// </summary>
public class NPCBehaviour : MonoBehaviour
{
    //Villager attached to this same gameobject
    private Villager m_villager;

    [TextArea, InspectorName("Activity"), 
        Tooltip("What am I doing right now")]
    
    public string m_activity;

    [SerializeField, 
        Tooltip("Assigned Job, can be hard coded or they will look for an open job if they need money")]
    private Occupation m_occupation;
    //collection of Want components
    private List<Want> m_wants = new List<Want>();
    //Dynamic Dictionary of locations that generate resources
    List<Location> m_locations = new List<Location>();

    private List<GOAPStep> m_plan = new List<GOAPStep>();
    private GOAPStep m_currentStep = null;


    private void Start()
    {
        m_villager = GetComponent<Villager>();
        //Check for existing wants
        m_wants.AddRange(GetComponents<Want>());
        //set up a randomized time to check for wants
        var timeSpace = UnityEngine.Random.Range(0, .5f);
        InvokeRepeating("CheckWants", 0.0f, 1.0f + timeSpace);
    }

    //called from start on wants when a new want is added to the player
    internal void AddWant(Want want)
    {
        if(!m_wants.Contains(want))
            m_wants.Add(want);
    }

    private void CheckWants()
    {
        m_activity = string.Empty;
        //sort wants by calculated need
        m_wants.Sort((Want a, Want b) => 
        {
            return a.m_calculatedNeed.CompareTo(b.m_calculatedNeed);
        });
        foreach (var want in m_wants)
        {
            //post to the Activity log
            m_activity += string.Format("Want: {0}  = {1}", want.GetName(), want.m_calculatedNeed);
        }
        if (m_wants.Count > 0)
            //find a location for my top priority
            FindLocation();
    }

    //Identify locations that the NPC is aware of that generate resources for thier need
    private void FindLocation()
    {
        //clear the pending Resource needs
        m_villager.ClearPendingResourceNeeds();
        m_plan.Clear();
        var topWant = m_wants[0].GetDesireable();
        var wantAmount = m_wants[0].GetDesiredAmount();
        FindLocationFor(topWant, wantAmount, transform.position);
        //If I have a valid recursive plan execute it
        if(m_plan.Count > 0)
        {
            StartCoroutine(ExecutePlan());
        }
    }

    private IEnumerator ExecutePlan()
    {
        yield return null;
        //while there are still steps needed to be taken
        while(m_plan.Count > 0)
        {
            //check if I have satisfied this step
            if(m_villager.HasReservedResource(m_plan[0].m_resourceType))
            {
                //then I have finished this step
                m_plan.RemoveAt(0);
            }
            //otherwise, go to the location for this step and trade resources for what I need

            else
            {
                StartCoroutine(PerfromStep());
                //wait until I have finished this step
                yield return new WaitUntil(()=> m_currentStep == null );
            }
        }

    }

    private IEnumerator PerfromStep()
    {
        m_currentStep = m_plan[0];

        //move to destination
        var destination = m_villager.Move(m_currentStep.m_location.gameObject);
        yield return new WaitUntil(() => m_villager.ArrivedAtDestination(destination));
        //transact with location
        //keep transacting with destination until I have all the resources I need
        while (m_villager.HasReservedResource(m_currentStep.m_resourceType))
        {
            //?????
        }
        //when I have finished, return and clear the current step
        m_currentStep = null;
        yield break;


    }


    //recursively build a plan to get the resources that I need
    private void FindLocationFor(Resource.ResourceType resource, int wantAmount, Vector3 position)
    {
        //if I have the resources to satisfy this then return this location
        var missingAmount = m_villager.ReserveResource(resource, wantAmount); 
        if(missingAmount <= 0)
        {
            m_plan.Add(new GOAPStep() { m_location = null, m_amount = wantAmount, m_resourceType = resource });
            return ;
        }
        //otherwise find a location that services this resource
        else
        {
            //generate a list of locations
            List<Location> locations = new List<Location>();
            foreach(var location in m_locations)
            {
                //if this location generates 
                if (location.GetResource() == resource)
                {
                    locations.Add(location);
                }
            }

            //in the event that there are not locations that I know of to statisfy my need

            if(locations.Count == 0)
            {
                m_plan.Clear();
                Debug.LogError("Could not find a resource generator for " + resource.ToString());
                return;
            }



            //sort the locations by distance to the position we will start looking
            //with the closes poosition being first

            locations.Sort((Location a, Location b) =>
            {
               
                if (a == null && b == null) return 0;           //if they are null then return 0
                else if (a == null) return 1;                  //if a is null then return 1
                else if (b == null) return -1;                   //if b is null then return -1
                else                                            //otherwise return a comparison of distances
                    return -1 *(Vector3.Distance(position, a.m_location).CompareTo(Vector3.Distance(position, b.m_location)));
            });

            var closestLocation = locations[0];
            //what does the location need in exchange for my want
            var need = closestLocation.GetResourceNeeded();
            var amount = closestLocation.GetAmountNeeded();
            FindLocationFor(need, amount, closestLocation.m_location);
            //then add this location 
            m_plan.Add(new GOAPStep() { m_location = closestLocation, m_amount = amount, m_resourceType = need });
            return;

        } 

    }   
}

internal class GOAPStep
{
    public Resource.ResourceType m_resourceType;
    public int m_amount;
    public Location m_location;
}
