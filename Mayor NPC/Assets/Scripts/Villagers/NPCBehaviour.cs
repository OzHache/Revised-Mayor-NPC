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
    //Dynamic list of locations that generate resources
    List<Location> m_locations = new List<Location>();

    private List<GOAPStep> m_plan = new List<GOAPStep>();
    private GOAPStep m_currentStep = null;


    private void Start()
    {
        m_locations.AddRange(FindObjectsOfType<Location>());
        m_villager = GetComponent<Villager>();
        //Check for existing wants
        m_wants.AddRange(GetComponents<Want>());
        //set up a randomized time to check for wants
        var timeSpace = UnityEngine.Random.Range(0, .5f);
        CheckWants();
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
        m_activity += string.Format("\nFinding location for {0}", m_wants[0].GetName());
        //clear the pending Resource needs
        m_villager.ClearPendingResourceNeeds();
        m_plan.Clear();
        var topWant = m_wants[0].GetDesireable();
        var wantAmount = m_wants[0].GetDesiredAmount();
        //If I have a valid recursive plan execute it
        if (FindLocationFor(topWant, wantAmount, transform.position))
        {
            m_activity += string.Format("\n found a plan for {0}", m_wants[0].GetName());
            StartCoroutine(ExecutePlan());
        }
        else
        {
            m_activity += string.Format("\n Could not find a plan for {0}", m_wants[0].GetName());
        }
    }

    private IEnumerator ExecutePlan()
    {
        yield return null;
        //while there are still steps needed to be taken
        while(m_plan.Count > 0)
        {
            //check if I have satisfied this step
            if(m_villager.HasReservedResource(m_plan[0].m_location.GetResource()))
            {
                m_activity += "\nFinshed step";
                //then I have finished this step
                m_plan.RemoveAt(0);
            }
            //otherwise, go to the location for this step and trade resources for what I need

            else
            {
                m_activity += "\nStarting step";
                StartCoroutine(PerfromStep());
                //wait until I have finished this step
                yield return new WaitUntil(()=> m_currentStep == null );
            }
        }

    }

    private IEnumerator PerfromStep()
    {
        m_currentStep = m_plan[0];
        string message = string.Format("\nPerforming Step: Resource {0}, in the amount of {1}", m_currentStep.m_resourceType.ToString(), m_currentStep.m_amount.ToString());
        m_activity += message;

        //move to destination
        var destination = m_villager.Move(m_currentStep.m_location.GetTransactionLocation());
        yield return new WaitUntil(() => m_villager.ArrivedAtDestination(destination));
        //the resource this location needs
        var resourceNeeded = m_currentStep.m_location.GetResourceNeeded();


        //transact with location
        //keep transacting with destination until I have all the resources I need
        while (!m_villager.HasReservedResource(m_currentStep.m_location.GetResource()))
        {
            //first check if the only resource needed is work
            if(resourceNeeded == Resource.ResourceType.k_work) 
            {
                //how many transactions required for the resources I need
                int transactions = m_currentStep.m_location.GetAmountNeededToFulfill(m_villager.GetReservationsFor(resourceNeeded));
                //how much work is required per transaction
                int work = m_currentStep.m_location.GetResourceRequiredPerTransaction();
                int amountPerTransAction = m_currentStep.m_location.GetResourceRequiredPerTransaction();
                for(var transaction = 0; transaction < transactions; transaction++)
                {
                    //wait for work seconds
                    m_activity += string.Format("\nWorking for {0} seconds", work);
                    yield return new WaitForSeconds(work);
                    var workAmount = work;
                    //work and add the resources to the villager
                    m_currentStep.m_location.Transact(amountPerTransAction, ref workAmount, out int resourceGenerated);
                    m_activity += string.Format("\nAdding {0} resource of {1} type to the npc", resourceGenerated, m_currentStep.m_location.GetResource());
                    m_villager.AddResource(m_currentStep.m_location.GetResource(), resourceGenerated);
                    
                }

                break;
            }

            //get all available resource of this type
            int allOfResource = m_villager.GetAllOfResource(resourceNeeded);
            //amount needed
            int amountNeeded = m_villager.GetReservationsFor(m_currentStep.m_resourceType);
            
            
            
            //Attempt to transact for the amount required


            if (m_currentStep.m_location.Transact(amountNeeded, ref allOfResource, out int amountReturned))
            {
                //push new resources
                m_villager.AddResource(m_currentStep.m_location.GetResource(), amountReturned);
            }
            else
            {
                //error because we did not have enough for the transaction This should not happen
                message = string.Format("\nAn error has occured for {0} resource. We failt to satisfy a reservation of {1} because we needed {2} of {3} but only had {4}"
                    , m_currentStep.m_resourceType.ToString()
                    , m_villager.GetReservationsFor(m_currentStep.m_resourceType).ToString()
                    , amountNeeded.ToString()
                    , resourceNeeded.ToString()
                    , allOfResource.ToString());

                Debug.LogError(message);
                //make a reservation for the missing amount
                int shortAmount = amountNeeded - allOfResource;
                //
                var _ = m_villager.ReserveResource(resourceNeeded, shortAmount);
                if (FindLocationFor(resourceNeeded, shortAmount, transform.position))
                {
                    //move the new location to the top of my plan
                    var step = m_plan[m_plan.Count - 1];
                    m_plan.Remove(step);
                    m_plan.Insert(0, step);
                }
                else 
                {
                    Debug.LogError("Cannot find a valid plan for this resource");
                }

            }
            //return unused resources
            if(allOfResource > 0)
            {
                m_villager.AddResource(resourceNeeded, amountReturned);
            }
        }
        //when I have finished, return and clear the current step
        m_currentStep = null;
        yield break;


    }


    //recursively build a plan to get the resources that I need
    private bool FindLocationFor(Resource.ResourceType resource, int wantAmount, Vector3 position)
    {
        //if this resource is work then I am good to go
        if(resource == Resource.ResourceType.k_work)
        {
            m_villager.ReserveResource(resource, wantAmount);
            m_activity += "\nAll that is required is work";
            return true;
        }

        //if I have the resources to satisfy this then return this location
        var missingAmount = m_villager.ReserveResource(resource, wantAmount);

        string message = string.Format("\nI have a need for {0}, in the amount of {1}, I am short {2}", resource.ToString(), wantAmount.ToString(), missingAmount.ToString());
        m_activity += message;

        

        if(missingAmount >= 0)
        {
            message = string.Format("\n I have everything I need so I am adding this step");
            m_activity += message;
            m_plan.Add(new GOAPStep() { m_location = null, m_amount = wantAmount, m_resourceType = resource });
            return true;
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
                   /* message = string.Format("\nFound a location for {0} at {1}", resource, location.transform.position.ToString());
                    m_activity += message;*/
                    locations.Add(location);
                }
            }

            //in the event that there are not locations that I know of to statisfy my need

            if(locations.Count == 0)
            {
                message = string.Format("\nUnable to find a location for resource {0}", resource.ToString());
                m_activity += message;
                m_plan.Clear();
                Debug.LogError("Could not find a resource generator for " + resource.ToString());
                return false;
            }
            //sort the locations by distance to the position we will start looking
            //with the closes poosition being first

            locations.Sort((Location a, Location b) =>
            {
               
                if (a == null && b == null) return 0;           //if they are null then return 0
                else if (a == null) return 1;                  //if a is null then return 1
                else if (b == null) return -1;                   //if b is null then return -1
                else                                            //otherwise return a comparison of distances
                    return (Vector3.Distance(position, a.GetTransactionLocation()).CompareTo(Vector3.Distance(position, b.GetTransactionLocation())));
            });

            var closestLocation = locations[0];
            //what does the location need in exchange for my want
            var need = closestLocation.GetResourceNeeded();
            var amount = closestLocation.GetAmountNeededToFulfill(m_villager.GetReservationsFor(resource));
            FindLocationFor(need, amount, closestLocation.GetTransactionLocation());
            //then add this location 
            message = string.Format("\nFound a location for {0} at {1}", resource, closestLocation.transform.position.ToString());
            m_activity += message;
            m_plan.Add(new GOAPStep() { m_location = closestLocation, m_amount = amount, m_resourceType = need });
            return true;

        } 

    }   
}

internal class GOAPStep
{
    public Resource.ResourceType m_resourceType;
    public int m_amount;
    public Location m_location;
}
